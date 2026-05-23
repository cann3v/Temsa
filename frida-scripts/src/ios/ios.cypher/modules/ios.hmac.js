import { IOS_CIPHER_CONFIG } from "../config.js";
import { CCHmacAlgorithm, CCHmacDigestLength } from "../constants/commoncrypto.js";
import { attachExport, isNull, toInt, toSize } from "../utils/native.js";
import { readByteArraySafe } from "../utils/data.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { enumName } from "../utils/format.js";

const hmacContexts = new Map();

export function installIosHmacHooks({ emit, config = IOS_CIPHER_CONFIG.hmac }) {
  if (!config.enabled) return;
  hookCCHmac(emit, config);
  hookCCHmacInit(emit, config);
  hookCCHmacUpdate(emit, config);
  hookCCHmacFinal(emit, config);
}

function hookCCHmac(emit, config) {
  attachExport("CCHmac", {
    onEnter(args) {
      this.algorithm = toInt(args[0]);
      this.digestLength = CCHmacDigestLength[this.algorithm] || 0;
      this.dataOut = args[5];
      this.event = {
        type: "telemetry.event",
        category: "crypto",
        event: "commoncrypto.hmac.oneshot",
        api: "CCHmac",
        operation: "hmac",
        algorithm: enumName(CCHmacAlgorithm, this.algorithm),
        key: dataField(args[1], toSize(args[2]), config),
        input: dataField(args[3], toSize(args[4]), config),
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      };
    },
    onLeave() {
      emit({
        ...this.event,
        digest: dataField(this.dataOut, this.digestLength, config)
      });
    }
  }, emit);
}

function hookCCHmacInit(emit, config) {
  attachExport("CCHmacInit", {
    onEnter(args) {
      const ctx = args[0];
      const algorithm = toInt(args[1]);
      const state = {
        algorithm: enumName(CCHmacAlgorithm, algorithm),
        totalInputLength: 0,
        key: dataField(args[2], toSize(args[3]), config)
      };
      if (!isNull(ctx)) hmacContexts.set(ctx.toString(), state);
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "commoncrypto.hmac.init",
        api: "CCHmacInit",
        context: ctx.toString(),
        ...state,
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
    }
  }, emit);
}

function hookCCHmacUpdate(emit, config) {
  attachExport("CCHmacUpdate", {
    onEnter(args) {
      const ctx = args[0];
      const length = toSize(args[2]);
      const state = !isNull(ctx) ? hmacContexts.get(ctx.toString()) || null : null;
      if (state !== null) {
        state.totalInputLength += length;
        hmacContexts.set(ctx.toString(), state);
      }
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "commoncrypto.hmac.update",
        api: "CCHmacUpdate",
        context: ctx.toString(),
        hmacState: state,
        input: dataField(args[1], length, config),
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
    }
  }, emit);
}

function hookCCHmacFinal(emit, config) {
  attachExport("CCHmacFinal", {
    onEnter(args) {
      this.ctx = args[0];
      this.macOut = args[1];
      this.state = !isNull(this.ctx) ? hmacContexts.get(this.ctx.toString()) || null : null;
    },
    onLeave() {
      let digestLength = 0;
      if (this.state !== null) {
        const algName = this.state.algorithm;
        Object.keys(CCHmacAlgorithm).forEach(k => {
          if (CCHmacAlgorithm[k] === algName) digestLength = CCHmacDigestLength[k] || 0;
        });
      }
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "commoncrypto.hmac.final",
        api: "CCHmacFinal",
        context: this.ctx.toString(),
        hmacState: this.state,
        digest: dataField(this.macOut, digestLength, config),
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
      if (!isNull(this.ctx)) hmacContexts.delete(this.ctx.toString());
    }
  }, emit);
}

function dataField(address, length, config) {
  return config.includeData ? readByteArraySafe(address, length, config.maxInputDataLength) : { length };
}
