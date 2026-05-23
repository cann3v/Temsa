import { IOS_CIPHER_CONFIG } from "../config.js";
import { CCAlgorithm, CCAlgorithmKey, CCMode, CCModeOptions, CCOperation, CCOptions, CCPadding } from "../constants/commoncrypto.js";
import { attachExport, isNull, toInt, toSize, toUInt } from "../utils/native.js";
import { readByteArraySafe, readSizeTPtr } from "../utils/data.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { enumName, optionNames } from "../utils/format.js";

const cryptorState = new Map();
let nextSessionId = 1;

export function installIosCryptoHooks({ emit, config = IOS_CIPHER_CONFIG.crypto }) {
  if (!config.enabled) return;
  hookCCCrypt(emit, config);
  hookCCCryptorCreate(emit, config);
  hookCCCryptorCreateWithMode(emit, config);
  hookCCCryptorUpdate(emit, config);
  hookCCCryptorFinal(emit, config);
}

function isAlgorithmEnabled(algorithm, config) {
  const key = CCAlgorithmKey[algorithm];
  return key === undefined || config.algorithms[key] !== false;
}

function dataField(address, length, config) {
  return config.includeData ? readByteArraySafe(address, length, config.maxDataLength) : { length };
}

function operationName(operation) {
  if (operation === 0) return "encrypt";
  if (operation === 1) return "decrypt";
  return enumName(CCOperation, operation);
}

function cryptoIoFields(operation, input, output, outputPrefix) {
  if (operation === "encrypt") {
    return outputPrefix === "final" ? { finalCiphertextChunk: output } : { plaintext: input, ciphertext: output };
  }
  if (operation === "decrypt") {
    return outputPrefix === "final" ? { finalPlaintextChunk: output } : { ciphertext: input, plaintext: output };
  }
  return outputPrefix === "final" ? { finalOutputChunk: output } : { input, output };
}

function hookCCCrypt(emit, config) {
  attachExport("CCCrypt", {
    onEnter(args) {
      const operationRaw = toInt(args[0]);
      const algorithm = toInt(args[1]);
      const options = toInt(args[2]);
      if (!isAlgorithmEnabled(algorithm, config)) {
        this.skip = true;
        return;
      }
      this.operation = operationName(operationRaw);
      this.out = args[8];
      this.outLengthPtr = args[10];
      this.input = dataField(args[6], toSize(args[7]), config);
      this.base = {
        type: "telemetry.event",
        category: "crypto",
        event: "crypto." + this.operation,
        api: "CCCrypt",
        operation: this.operation,
        operationConstant: enumName(CCOperation, operationRaw),
        algorithm: enumName(CCAlgorithm, algorithm),
        options: optionNames(CCOptions, options),
        key: dataField(args[3], toSize(args[4]), config),
        iv: dataField(args[5], blockIvGuessLength(algorithm), config),
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      };
    },
    onLeave(retval) {
      if (this.skip) return;
      const outLength = readSizeTPtr(this.outLengthPtr);
      const output = outLength !== null ? dataField(this.out, outLength, config) : null;
      emit({
        ...this.base,
        ...cryptoIoFields(this.operation, this.input, output),
        status: toInt(retval)
      });
    }
  }, emit);
}

function hookCCCryptorCreate(emit, config) {
  attachExport("CCCryptorCreate", {
    onEnter(args) {
      const operationRaw = toInt(args[0]);
      const algorithm = toInt(args[1]);
      const options = toInt(args[2]);
      if (!isAlgorithmEnabled(algorithm, config)) {
        this.skip = true;
        return;
      }
      this.cryptorRefPtr = args[6];
      this.state = {
        sessionId: "ccryptor-" + nextSessionId++,
        operation: operationName(operationRaw),
        operationConstant: enumName(CCOperation, operationRaw),
        algorithm: enumName(CCAlgorithm, algorithm),
        options: optionNames(CCOptions, options),
        key: dataField(args[3], toSize(args[4]), config),
        iv: dataField(args[5], blockIvGuessLength(algorithm), config)
      };
    },
    onLeave(retval) {
      if (this.skip) return;
      const cryptorRef = readPointerSafe(this.cryptorRefPtr);
      if (cryptorRef !== null) cryptorState.set(cryptorRef.toString(), this.state);
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "crypto.session.created",
        api: "CCCryptorCreate",
        status: toInt(retval),
        cryptor: cryptorRef !== null ? cryptorRef.toString() : null,
        ...this.state,
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
    }
  }, emit);
}

function hookCCCryptorCreateWithMode(emit, config) {
  attachExport("CCCryptorCreateWithMode", {
    onEnter(args) {
      const operationRaw = toInt(args[0]);
      const mode = toInt(args[1]);
      const algorithm = toInt(args[2]);
      const padding = toInt(args[3]);
      const modeOptions = toUInt(args[8]);
      if (!isAlgorithmEnabled(algorithm, config)) {
        this.skip = true;
        return;
      }
      this.cryptorRefPtr = args[10];
      this.state = {
        sessionId: "ccryptor-" + nextSessionId++,
        operation: operationName(operationRaw),
        operationConstant: enumName(CCOperation, operationRaw),
        mode: enumName(CCMode, mode),
        algorithm: enumName(CCAlgorithm, algorithm),
        padding: enumName(CCPadding, padding),
        modeOptions: optionNames(CCModeOptions, modeOptions),
        key: dataField(args[5], toSize(args[6]), config),
        iv: dataField(args[4], blockIvGuessLength(algorithm), config),
        tweak: dataField(args[7], blockIvGuessLength(algorithm), config)
      };
    },
    onLeave(retval) {
      if (this.skip) return;
      const cryptorRef = readPointerSafe(this.cryptorRefPtr);
      if (cryptorRef !== null) cryptorState.set(cryptorRef.toString(), this.state);
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "crypto.session.created",
        api: "CCCryptorCreateWithMode",
        status: toInt(retval),
        cryptor: cryptorRef !== null ? cryptorRef.toString() : null,
        ...this.state,
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
    }
  }, emit);
}

function hookCCCryptorUpdate(emit, config) {
  attachExport("CCCryptorUpdate", {
    onEnter(args) {
      this.cryptor = args[0];
      this.out = args[3];
      this.outLengthPtr = args[5];
      this.input = dataField(args[1], toSize(args[2]), config);
      this.state = !isNull(this.cryptor) ? cryptorState.get(this.cryptor.toString()) || null : null;
    },
    onLeave(retval) {
      const outLength = readSizeTPtr(this.outLengthPtr);
      const output = outLength !== null ? dataField(this.out, outLength, config) : null;
      const operation = this.state && this.state.operation ? this.state.operation : "unknown";
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "crypto.session.update",
        api: "CCCryptorUpdate",
        status: toInt(retval),
        cryptor: isNull(this.cryptor) ? null : this.cryptor.toString(),
        sessionId: this.state ? this.state.sessionId : null,
        operation,
        algorithm: this.state ? this.state.algorithm : null,
        mode: this.state ? this.state.mode : null,
        ...cryptoIoFields(operation, this.input, output),
        cryptorState: this.state,
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
    }
  }, emit);
}

function hookCCCryptorFinal(emit, config) {
  attachExport("CCCryptorFinal", {
    onEnter(args) {
      this.cryptor = args[0];
      this.out = args[1];
      this.outLengthPtr = args[3];
      this.state = !isNull(this.cryptor) ? cryptorState.get(this.cryptor.toString()) || null : null;
    },
    onLeave(retval) {
      const outLength = readSizeTPtr(this.outLengthPtr);
      const output = outLength !== null ? dataField(this.out, outLength, config) : null;
      const operation = this.state && this.state.operation ? this.state.operation : "unknown";
      emit({
        type: "telemetry.event",
        category: "crypto",
        event: "crypto.session.finalized",
        api: "CCCryptorFinal",
        status: toInt(retval),
        cryptor: isNull(this.cryptor) ? null : this.cryptor.toString(),
        sessionId: this.state ? this.state.sessionId : null,
        operation,
        algorithm: this.state ? this.state.algorithm : null,
        mode: this.state ? this.state.mode : null,
        ...cryptoIoFields(operation, null, output, "final"),
        cryptorState: this.state,
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      });
      if (!isNull(this.cryptor)) cryptorState.delete(this.cryptor.toString());
    }
  }, emit);
}

function blockIvGuessLength(algorithm) {
  if (algorithm === 0) return 16;
  if (algorithm === 2) return 8;
  if (algorithm === 1 || algorithm === 3 || algorithm === 5 || algorithm === 6) return 8;
  return 0;
}

function readPointerSafe(address) {
  if (isNull(address)) return null;
  try {
    const p = address.readPointer();
    return p.isNull() ? null : p;
  } catch (_) {
    return null;
  }
}
