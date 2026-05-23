export function findExport(name) {
  try {
    return Module.findGlobalExportByName(name);
  } catch (_) {
    return null;
  }
}

export function attachExport(name, callbacks, emit) {
  const address = findExport(name);
  if (address === null) {
    emit && emit({
      type: "telemetry.event",
      category: "instrumentation",
      event: "export.missing",
      api: name
    });
    return false;
  }
  Interceptor.attach(address, callbacks);
  emit && emit({
    type: "telemetry.event",
    category: "instrumentation",
    event: "export.hooked",
    api: name,
    address: address.toString()
  });
  return true;
}

export function toInt(value) {
  try {
    return value.toInt32();
  } catch (_) {
    return Number(value);
  }
}

export function toUInt(value) {
  try {
    return value.toUInt32();
  } catch (_) {
    return Number(value);
  }
}

export function toSize(value) {
  if (value === undefined || value === null) return 0;
  try {
    const n = value.toUInt32();
    return n < 0 ? 0 : n;
  } catch (_) {
    const n = Number(value);
    return Number.isFinite(n) && n > 0 ? n : 0;
  }
}

export function isNull(ptrValue) {
  return ptrValue === null || ptrValue.isNull();
}
