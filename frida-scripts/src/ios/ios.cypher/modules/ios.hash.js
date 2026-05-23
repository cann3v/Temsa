import { IOS_CIPHER_CONFIG } from "../config.js";
import { HashDigestLength } from "../constants/commoncrypto.js";
import { attachExport, isNull, toSize } from "../utils/native.js";
import { readByteArraySafe } from "../utils/data.js";
import { captureBacktrace } from "../utils/backtrace.js";

const HASHES = ["CC_MD2", "CC_MD4", "CC_MD5", "CC_SHA1", "CC_SHA224", "CC_SHA256", "CC_SHA384", "CC_SHA512"];
const hashContexts = new Map();

export function installIosHashHooks({ emit, config = IOS_CIPHER_CONFIG.hash }) {
  if (!config.enabled) return;
  HASHES.forEach(name => {
    const configKey = name.replace("CC_", "").toLowerCase();
    if (config.algorithms[configKey] === false) return;
    hookOneShotHash(name, emit, config);
    hookStreamingHash(name, emit, config);
  });
}

function hookOneShotHash(name, emit, config) {
  attachExport(name, {
    onEnter(args) {
      this.data = args[0];
      this.length = toSize(args[1]);
      this.digest = args[2];
    },
    onLeave(retval) {
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "commoncrypto.hash.oneshot",
        api: name,
        operation: "hash",
        algorithm: name,
        input: dataField(this.data, this.length, config),
        digest: dataField(this.digest, HashDigestLength[name], config),
        returnValue: retval.toString(),
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
    }
  }, emit);
}

function hookStreamingHash(name, emit, config) {
  attachExport(name + "_Init", {
    onEnter(args) {
      const ctx = args[0];
      if (!isNull(ctx)) hashContexts.set(ctx.toString(), { algorithm: name, totalInputLength: 0 });
      emit({ type: "telemetry.event", category: "crypto", event: "commoncrypto.hash.init", api: name + "_Init", algorithm: name, context: ctx.toString() });
    }
  }, emit);

  attachExport(name + "_Update", {
    onEnter(args) {
      const ctx = args[0];
      const length = toSize(args[2]);
      const state = !isNull(ctx) ? hashContexts.get(ctx.toString()) || { algorithm: name, totalInputLength: 0 } : null;
      if (state !== null) {
        state.totalInputLength += length;
        hashContexts.set(ctx.toString(), state);
      }
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "commoncrypto.hash.update",
        api: name + "_Update",
        algorithm: name,
        context: ctx.toString(),
        input: dataField(args[1], length, config),
        totalInputLength: state !== null ? state.totalInputLength : null,
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
    }
  }, emit);

  attachExport(name + "_Final", {
    onEnter(args) {
      this.digest = args[0];
      this.ctx = args[1];
      this.state = !isNull(this.ctx) ? hashContexts.get(this.ctx.toString()) || null : null;
    },
    onLeave(retval) {
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "commoncrypto.hash.final",
        api: name + "_Final",
        algorithm: name,
        context: this.ctx.toString(),
        totalInputLength: this.state !== null ? this.state.totalInputLength : null,
        digest: dataField(this.digest, HashDigestLength[name], config),
        status: retval.toString(),
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
      if (!isNull(this.ctx)) hashContexts.delete(this.ctx.toString());
    }
  }, emit);
}

function dataField(address, length, config) {
  return config.includeData ? readByteArraySafe(address, length, config.maxInputDataLength) : { length };
}
