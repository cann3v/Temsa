import { IOS_CIPHER_CONFIG } from "../config.js";
import { CCPBKDFAlgorithm, CCPseudoRandomAlgorithm } from "../constants/commoncrypto.js";
import { attachExport, toInt, toSize, toUInt } from "../utils/native.js";
import { readByteArraySafe, readUtf8StringSafe } from "../utils/data.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { enumName } from "../utils/format.js";

export function installIosPbkdfHooks({ emit, config = IOS_CIPHER_CONFIG.pbkdf }) {
  if (!config.enabled) return;
  hookCCKeyDerivationPBKDF(emit, config);
  hookCCCalibratePBKDF(emit, config);
}

function hookCCKeyDerivationPBKDF(emit, config) {
  attachExport("CCKeyDerivationPBKDF", {
    onEnter(args) {
      this.derivedKey = args[8];
      this.derivedKeyLen = toSize(args[9]);
      this.event = {
        type: "telemetry.event",
        category: "crypto",
        event: "commoncrypto.pbkdf.derive",
        api: "CCKeyDerivationPBKDF",
        algorithm: enumName(CCPBKDFAlgorithm, toInt(args[0])),
        password: readUtf8StringSafe(args[1], toSize(args[2])),
        passwordLength: toSize(args[2]),
        salt: readByteArraySafe(args[3], toSize(args[4]), 240),
        prf: enumName(CCPseudoRandomAlgorithm, toInt(args[5])),
        rounds: toUInt(args[6]),
        derivedKeyLength: this.derivedKeyLen,
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      };
    },
    onLeave(retval) {
      emit({
        ...this.event,
        status: toInt(retval),
        derivedKey: readByteArraySafe(this.derivedKey, this.derivedKeyLen, 240)
      });
    }
  }, emit);
}

function hookCCCalibratePBKDF(emit, config) {
  attachExport("CCCalibratePBKDF", {
    onEnter(args) {
      this.event = {
        type: "telemetry.event",
        category: "crypto",
        event: "commoncrypto.pbkdf.calibrate",
        api: "CCCalibratePBKDF",
        algorithm: enumName(CCPBKDFAlgorithm, toInt(args[0])),
        passwordLength: toSize(args[1]),
        saltLength: toSize(args[2]),
        prf: enumName(CCPseudoRandomAlgorithm, toInt(args[3])),
        derivedKeyLength: toSize(args[4]),
        milliseconds: toUInt(args[5]),
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      };
    },
    onLeave(retval) {
      emit({ ...this.event, rounds: toUInt(retval) });
    }
  }, emit);
}
