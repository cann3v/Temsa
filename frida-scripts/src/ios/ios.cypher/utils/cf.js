import { findExport, isNull } from "./native.js";
import { readByteArraySafe, readCStringSafe } from "./data.js";

let initialized = false;
const api = {};

const kCFStringEncodingUTF8 = 0x08000100;
const kCFNumberSInt64Type = 4;

const KEYCHAIN_KEY_NAMES = {
  class: "class",
  acct: "account",
  svce: "service",
  agrp: "accessGroup",
  gena: "generic",
  labl: "label",
  desc: "description",
  cdat: "creationDate",
  mdat: "modificationDate",
  invi: "invisible",
  nega: "negative",
  sync: "synchronizable",
  pdmn: "accessible",
  v_Data: "valueData",
  v_Ref: "valueRef",
  v_PRef: "valuePersistentRef",
  r_Data: "returnData",
  r_Ref: "returnRef",
  r_Attrs: "returnAttributes",
  r_PRef: "returnPersistentRef",
  m_Limit: "matchLimit",
  m_LimitAll: "matchLimitAll",
  m_LimitOne: "matchLimitOne",
  m_CaseInsensitive: "matchCaseInsensitive",
  m_EmailAddressIfPresent: "matchEmailAddressIfPresent",
  m_SubjectContains: "matchSubjectContains",
  m_IssuerContains: "matchIssuerContains",
  m_SerialNumber: "matchSerialNumber",
  m_Policy: "matchPolicy",
  m_TrustedOnly: "matchTrustedOnly",
  m_ValidOnDate: "matchValidOnDate",
  m_TrustedCertificates: "matchTrustedCertificates",
  pdmn: "accessible"
};

const KEYCHAIN_VALUE_NAMES = {
  genp: "kSecClassGenericPassword",
  inet: "kSecClassInternetPassword",
  cert: "kSecClassCertificate",
  keys: "kSecClassKey",
  idnt: "kSecClassIdentity",
  ak: "kSecAttrAccessibleWhenUnlocked",
  ck: "kSecAttrAccessibleAfterFirstUnlock",
  dk: "kSecAttrAccessibleAlways",
  aku: "kSecAttrAccessibleWhenUnlockedThisDeviceOnly",
  cku: "kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly",
  dku: "kSecAttrAccessibleAlwaysThisDeviceOnly",
  akpu: "kSecAttrAccessibleWhenPasscodeSetThisDeviceOnly",
  m_LimitOne: "kSecMatchLimitOne",
  m_LimitAll: "kSecMatchLimitAll"
};

function init() {
  if (initialized) return;
  initialized = true;

  bind("CFGetTypeID", "ulong", ["pointer"]);
  bind("CFCopyDescription", "pointer", ["pointer"]);
  bind("CFRelease", "void", ["pointer"]);

  bind("CFStringGetTypeID", "ulong", []);
  bind("CFStringGetCString", "bool", ["pointer", "pointer", "long", "uint32"]);

  bind("CFDataGetTypeID", "ulong", []);
  bind("CFDataGetLength", "long", ["pointer"]);
  bind("CFDataGetBytePtr", "pointer", ["pointer"]);

  bind("CFDictionaryGetTypeID", "ulong", []);
  bind("CFDictionaryGetCount", "long", ["pointer"]);
  bind("CFDictionaryGetKeysAndValues", "void", ["pointer", "pointer", "pointer"]);

  bind("CFArrayGetTypeID", "ulong", []);
  bind("CFArrayGetCount", "long", ["pointer"]);
  bind("CFArrayGetValueAtIndex", "pointer", ["pointer", "long"]);

  bind("CFBooleanGetTypeID", "ulong", []);
  bind("CFBooleanGetValue", "bool", ["pointer"]);

  bind("CFNumberGetTypeID", "ulong", []);
  bind("CFNumberGetValue", "bool", ["pointer", "int", "pointer"]);

  bind("CFNullGetTypeID", "ulong", []);
}

function bind(name, ret, args) {
  const p = findExport(name);
  if (p !== null) api[name] = new NativeFunction(p, ret, args);
}

function has(...names) {
  for (let i = 0; i < names.length; i++) if (!api[names[i]]) return false;
  return true;
}

function sameType(value, typeFnName) {
  try {
    return has("CFGetTypeID", typeFnName) && Number(api.CFGetTypeID(value)) === Number(api[typeFnName]());
  } catch (_) {
    return false;
  }
}

export function parseCfType(value, options = {}) {
  init();
  const maxDepth = options.maxDepth === undefined ? 5 : options.maxDepth;
  const maxDataLength = options.maxDataLength || 512;
  return parseAny(value, { maxDepth, maxDataLength, seen: {}, depth: 0 });
}

function parseAny(value, ctx) {
  if (value === null || value === undefined || isNull(value)) return null;
  const pointer = value.toString();

  if (ctx.seen[pointer]) return { type: "reference", pointer };
  if (ctx.depth > ctx.maxDepth) return { type: "max-depth", pointer, description: copyCfDescription(value) };

  ctx.seen[pointer] = true;
  try {
    if (sameType(value, "CFStringGetTypeID")) return parseString(value);
    if (sameType(value, "CFDataGetTypeID")) return parseData(value, ctx.maxDataLength);
    if (sameType(value, "CFDictionaryGetTypeID")) return parseDictionary(value, childCtx(ctx));
    if (sameType(value, "CFArrayGetTypeID")) return parseArray(value, childCtx(ctx));
    if (sameType(value, "CFBooleanGetTypeID")) return parseBoolean(value);
    if (sameType(value, "CFNumberGetTypeID")) return parseNumber(value);
    if (sameType(value, "CFNullGetTypeID")) return null;
    return { type: "unknown", pointer, description: copyCfDescription(value) };
  } catch (e) {
    return { type: "error", pointer, error: String(e), description: copyCfDescription(value) };
  } finally {
    delete ctx.seen[pointer];
  }
}

function childCtx(ctx) {
  return { maxDepth: ctx.maxDepth, maxDataLength: ctx.maxDataLength, seen: ctx.seen, depth: ctx.depth + 1 };
}

function parseString(value) {
  const stringValue = cfStringToJsString(value);
  return {
    type: "string",
    value: normalizeKeychainValue(stringValue),
    rawValue: stringValue
  };
}

function parseData(value, maxDataLength) {
  if (!has("CFDataGetLength", "CFDataGetBytePtr")) return { type: "data", pointer: value.toString(), description: copyCfDescription(value) };
  const length = Number(api.CFDataGetLength(value));
  const bytes = api.CFDataGetBytePtr(value);
  return {
    type: "data",
    value: readByteArraySafe(bytes, length, maxDataLength)
  };
}

function parseDictionary(value, ctx) {
  if (!has("CFDictionaryGetCount", "CFDictionaryGetKeysAndValues")) return { type: "dictionary", pointer: value.toString(), description: copyCfDescription(value) };
  const count = Number(api.CFDictionaryGetCount(value));
  const keys = Memory.alloc(Process.pointerSize * count);
  const values = Memory.alloc(Process.pointerSize * count);
  api.CFDictionaryGetKeysAndValues(value, keys, values);

  const object = {};
  const raw = [];
  for (let i = 0; i < count; i++) {
    const keyPtr = keys.add(i * Process.pointerSize).readPointer();
    const valuePtr = values.add(i * Process.pointerSize).readPointer();
    const keyString = sameType(keyPtr, "CFStringGetTypeID") ? cfStringToJsString(keyPtr) : String(copyCfDescription(keyPtr));
    const normalizedKey = normalizeKeychainKey(keyString);
    const parsedValue = parseAny(valuePtr, ctx);
    object[normalizedKey] = unwrapKeychainValue(parsedValue);
    raw.push({ key: keyString, normalizedKey, value: parsedValue });
  }

  return { type: "dictionary", count, value: object, raw };
}

function parseArray(value, ctx) {
  if (!has("CFArrayGetCount", "CFArrayGetValueAtIndex")) return { type: "array", pointer: value.toString(), description: copyCfDescription(value) };
  const count = Number(api.CFArrayGetCount(value));
  const out = [];
  for (let i = 0; i < count; i++) {
    out.push(unwrapKeychainValue(parseAny(api.CFArrayGetValueAtIndex(value, i), ctx)));
  }
  return { type: "array", count, value: out };
}

function parseBoolean(value) {
  if (!has("CFBooleanGetValue")) return { type: "boolean", pointer: value.toString(), description: copyCfDescription(value) };
  return { type: "boolean", value: !!api.CFBooleanGetValue(value) };
}

function parseNumber(value) {
  if (!has("CFNumberGetValue")) return { type: "number", pointer: value.toString(), description: copyCfDescription(value) };
  const out = Memory.alloc(8);
  const ok = api.CFNumberGetValue(value, kCFNumberSInt64Type, out);
  return ok ? { type: "number", value: Number(out.readS64()) } : { type: "number", pointer: value.toString(), description: copyCfDescription(value) };
}

function cfStringToJsString(value) {
  if (!has("CFStringGetCString")) return String(copyCfDescription(value));
  const capacity = 8192;
  const buffer = Memory.alloc(capacity);
  const ok = api.CFStringGetCString(value, buffer, capacity, kCFStringEncodingUTF8);
  return ok ? readCStringSafe(buffer) : String(copyCfDescription(value));
}

function unwrapKeychainValue(parsed) {
  if (parsed === null) return null;
  if (parsed.type === "string") return parsed.value;
  if (parsed.type === "boolean") return parsed.value;
  if (parsed.type === "number") return parsed.value;
  if (parsed.type === "data") return parsed.value;
  if (parsed.type === "array") return parsed.value;
  if (parsed.type === "dictionary") return parsed.value;
  return parsed;
}

function normalizeKeychainKey(key) {
  return KEYCHAIN_KEY_NAMES[key] || key;
}

function normalizeKeychainValue(value) {
  return KEYCHAIN_VALUE_NAMES[value] || value;
}

export function copyCfDescription(value) {
  init();
  if (!has("CFCopyDescription", "CFStringGetCString") || value === null || value === undefined || isNull(value)) {
    return null;
  }

  let desc = NULL;
  try {
    desc = api.CFCopyDescription(value);
    if (desc.isNull()) return null;
    return cfStringToJsString(desc);
  } catch (e) {
    return { error: String(e), pointer: value.toString() };
  } finally {
    if (desc !== NULL && !desc.isNull() && api.CFRelease) {
      try { api.CFRelease(desc); } catch (_) {}
    }
  }
}
