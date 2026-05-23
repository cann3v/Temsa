import { isNull } from "./native.js";

const HEX = "0123456789abcdef";

export function readByteArraySafe(address, length, maxLength) {
  if (address === null || address === undefined || isNull(address)) return null;
  const requestedLength = Math.max(0, Number(length) || 0);
  const effectiveLength = Math.min(requestedLength, maxLength || requestedLength);
  if (effectiveLength === 0) {
    return { length: requestedLength, capturedLength: 0, truncated: requestedLength > 0, hex: "", utf8: "" };
  }

  try {
    const bytes = address.readByteArray(effectiveLength);
    if (bytes === null) return null;
    return binaryField(bytes, requestedLength, effectiveLength);
  } catch (e) {
    return {
      length: requestedLength,
      capturedLength: 0,
      truncated: requestedLength > 0,
      error: String(e)
    };
  }
}

export function binaryField(buffer, requestedLength, capturedLength) {
  const effectiveLength = capturedLength === undefined ? buffer.byteLength : capturedLength;
  return {
    encoding: "hex+utf8-preview",
    length: requestedLength,
    capturedLength: effectiveLength,
    truncated: requestedLength > effectiveLength,
    hex: arrayBufferToHex(buffer),
    utf8Preview: arrayBufferToUtf8Preview(buffer)
  };
}

export function readUtf8StringSafe(address, maxLength) {
  if (address === null || address === undefined || isNull(address)) return null;
  try {
    if (maxLength !== undefined && maxLength !== null && maxLength >= 0) {
      const bytes = address.readByteArray(maxLength);
      return bytes === null ? null : arrayBufferToUtf8Preview(bytes);
    }
    return address.readUtf8String();
  } catch (e) {
    return { error: String(e), pointer: address.toString() };
  }
}

export function readCStringSafe(address) {
  if (address === null || address === undefined || isNull(address)) return null;
  try {
    return address.readUtf8String();
  } catch (e) {
    return { error: String(e), pointer: address.toString() };
  }
}

export function readSizeTPtr(address) {
  if (address === null || address === undefined || isNull(address)) return null;
  try {
    return Process.pointerSize === 8 ? Number(address.readU64()) : address.readU32();
  } catch (_) {
    return null;
  }
}

export function arrayBufferToHex(buffer) {
  const bytes = new Uint8Array(buffer);
  let out = "";
  for (let i = 0; i < bytes.length; i++) {
    const b = bytes[i];
    out += HEX[(b >>> 4) & 0xf] + HEX[b & 0xf];
  }
  return out;
}

function arrayBufferToUtf8Preview(buffer) {
  const bytes = new Uint8Array(buffer);
  if (bytes.length === 0) return "";

  // Avoid presenting obviously binary data as text, but allow normal whitespace.
  for (let i = 0; i < bytes.length; i++) {
    const b = bytes[i];
    if (b === 0 || b < 0x09 || (b > 0x0d && b < 0x20)) return null;
  }

  try {
    // TextDecoder is available in modern Frida runtimes. Keep a fallback for older runtimes.
    if (typeof TextDecoder !== "undefined") {
      return new TextDecoder("utf-8", { fatal: false }).decode(bytes);
    }
    let s = "";
    for (let i = 0; i < bytes.length; i++) s += String.fromCharCode(bytes[i]);
    return decodeURIComponent(escape(s));
  } catch (_) {
    return null;
  }
}
