export function captureBacktrace(context, enabled) {
  if (!enabled) return null;
  try {
    return Thread.backtrace(context, Backtracer.ACCURATE)
      .map(address => DebugSymbol.fromAddress(address).toString());
  } catch (e) {
    return ["<backtrace error: " + String(e) + ">"];
  }
}
