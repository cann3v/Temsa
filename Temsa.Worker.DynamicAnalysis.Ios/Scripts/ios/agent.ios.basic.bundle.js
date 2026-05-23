📦
1152 /src/ios/agent.ios.basic.js.map
949 /src/ios/agent.ios.basic.js
1171 /src/ios/ios.cypher/config.js.map
1227 /src/ios/ios.cypher/config.js
2054 /src/ios/ios.cypher/constants/commoncrypto.js.map
1807 /src/ios/ios.cypher/constants/commoncrypto.js
1788 /src/ios/ios.cypher/modules/ios.cipher.js.map
1546 /src/ios/ios.cypher/modules/ios.cipher.js
11143 /src/ios/ios.cypher/modules/ios.crypto.js.map
10804 /src/ios/ios.cypher/modules/ios.crypto.js
4651 /src/ios/ios.cypher/modules/ios.hash.js.map
4361 /src/ios/ios.cypher/modules/ios.hash.js
5064 /src/ios/ios.cypher/modules/ios.hmac.js.map
4980 /src/ios/ios.cypher/modules/ios.hmac.js
4565 /src/ios/ios.cypher/modules/ios.keychain.js.map
4505 /src/ios/ios.cypher/modules/ios.keychain.js
2833 /src/ios/ios.cypher/modules/ios.pbkdf.js.map
2808 /src/ios/ios.cypher/modules/ios.pbkdf.js
3404 /src/ios/ios.cypher/modules/ios.seckey.js.map
3416 /src/ios/ios.cypher/modules/ios.seckey.js
543 /src/ios/ios.cypher/utils/backtrace.js.map
324 /src/ios/ios.cypher/utils/backtrace.js
10445 /src/ios/ios.cypher/utils/cf.js.map
9770 /src/ios/ios.cypher/utils/cf.js
3782 /src/ios/ios.cypher/utils/data.js.map
3536 /src/ios/ios.cypher/utils/data.js
952 /src/ios/ios.cypher/utils/format.js.map
521 /src/ios/ios.cypher/utils/format.js
1677 /src/ios/ios.cypher/utils/native.js.map
1358 /src/ios/ios.cypher/utils/native.js
✄
{"version":3,"file":"agent.ios.basic.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/agent.ios.basic.js"],"names":[],"mappings":"AAAA,OAAO,EAAE,qBAAqB,EAAE,MAAM,oCAAoC,CAAC;AAE3E,MAAM,UAAU,GAAG,WAAW,CAAC;AAC/B,MAAM,cAAc,GAAG,CAAC,CAAC;AAEzB,SAAS,IAAI,CAAC,KAAK;IACjB,IAAI,CAAC;QACH,aAAa,EAAE,cAAc;QAC7B,SAAS,EAAE,UAAU;QACrB,QAAQ,EAAE,KAAK;QACf,SAAS,EAAE,IAAI,IAAI,EAAE,CAAC,WAAW,EAAE;QACnC,GAAG,KAAK;KACT,CAAC,CAAC;AACL,CAAC;AAED,SAAS,aAAa,CAAC,QAAQ,EAAE,SAAS;IACxC,IAAI;QACF,IAAI,CAAC,EAAE,IAAI,EAAE,mBAAmB,EAAE,QAAQ,EAAE,CAAC,CAAC;QAC9C,SAAS,CAAC;YACR,IAAI,EAAE,KAAK,CAAC,EAAE,CAAC,IAAI,CAAC;gBAClB,QAAQ;gBACR,GAAG,KAAK;aACT,CAAC;SACH,CAAC,CAAC;QACH,IAAI,CAAC,EAAE,IAAI,EAAE,kBAAkB,EAAE,QAAQ,EAAE,CAAC,CAAC;KAC9C;IAAC,OAAO,CAAC,EAAE;QACV,IAAI,CAAC;YACH,IAAI,EAAE,cAAc;YACpB,QAAQ;YACR,OAAO,EAAE,MAAM,CAAC,CAAC,CAAC;YAClB,KAAK,EAAE,CAAC,IAAI,CAAC,CAAC,KAAK,CAAC,CAAC,CAAC,MAAM,CAAC,CAAC,CAAC,KAAK,CAAC,CAAC,CAAC,CAAC,IAAI;SAC7C,CAAC,CAAC;KACJ;AACH,CAAC;AAED,IAAI,CAAC,EAAE,IAAI,EAAE,eAAe,EAAE,CAAC,CAAC;AAChC,aAAa,CAAC,YAAY,EAAE,qBAAqB,CAAC,CAAC;AACnD,IAAI,CAAC,EAAE,IAAI,EAAE,aAAa,EAAE,CAAC,CAAC"}
✄
import { installIosCipherHooks } from "./ios.cypher/modules/ios.cipher.js";
const PROFILE_ID = "ios.basic";
const SCHEMA_VERSION = 1;
function emit(event) {
    send({
        schemaVersion: SCHEMA_VERSION,
        profileId: PROFILE_ID,
        platform: "ios",
        timestamp: new Date().toISOString(),
        ...event
    });
}
function installModule(moduleId, installer) {
    try {
        emit({ type: "module.installing", moduleId });
        installer({
            emit: event => emit({
                moduleId,
                ...event
            })
        });
        emit({ type: "module.installed", moduleId });
    }
    catch (e) {
        emit({
            type: "module.error",
            moduleId,
            message: String(e),
            stack: e && e.stack ? String(e.stack) : null
        });
    }
}
emit({ type: "agent.started" });
installModule("ios.cipher", installIosCipherHooks);
emit({ type: "agent.ready" });
✄
{"version":3,"file":"config.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/config.js"],"names":[],"mappings":"AAAA,MAAM,CAAC,MAAM,iBAAiB,GAAG;IAC/B,MAAM,EAAE;QACN,OAAO,EAAE,KAAK;QACd,aAAa,EAAE,GAAG;QAClB,WAAW,EAAE,IAAI;QACjB,gBAAgB,EAAE,KAAK;QACvB,UAAU,EAAE;YACV,GAAG,EAAE,IAAI;YACT,GAAG,EAAE,IAAI;YACT,SAAS,EAAE,IAAI;YACf,IAAI,EAAE,IAAI;YACV,GAAG,EAAE,IAAI;YACT,GAAG,EAAE,IAAI;YACT,QAAQ,EAAE,IAAI;SACf;KACF;IACD,IAAI,EAAE;QACJ,OAAO,EAAE,KAAK;QACd,kBAAkB,EAAE,GAAG;QACvB,WAAW,EAAE,IAAI;QACjB,gBAAgB,EAAE,KAAK;QACvB,UAAU,EAAE;YACV,GAAG,EAAE,IAAI;YACT,GAAG,EAAE,IAAI;YACT,GAAG,EAAE,IAAI;YACT,IAAI,EAAE,IAAI;YACV,MAAM,EAAE,IAAI;YACZ,MAAM,EAAE,IAAI;YACZ,MAAM,EAAE,IAAI;YACZ,MAAM,EAAE,IAAI;SACb;KACF;IACD,IAAI,EAAE;QACJ,OAAO,EAAE,KAAK;QACd,kBAAkB,EAAE,GAAG;QACvB,WAAW,EAAE,IAAI;QACjB,gBAAgB,EAAE,KAAK;KACxB;IACD,KAAK,EAAE;QACL,OAAO,EAAE,KAAK;QACd,gBAAgB,EAAE,KAAK;KACxB;IACD,QAAQ,EAAE;QACR,OAAO,EAAE,IAAI;QACb,gBAAgB,EAAE,KAAK;QACvB,aAAa,EAAE,GAAG;QAClB,QAAQ,EAAE,CAAC;KACZ;IACD,MAAM,EAAE;QACN,OAAO,EAAE,IAAI;QACb,WAAW,EAAE,IAAI;QACjB,gBAAgB,EAAE,KAAK;QACvB,aAAa,EAAE,GAAG;QAClB,QAAQ,EAAE,CAAC;KACZ;CACF,CAAC"}
✄
export const IOS_CIPHER_CONFIG = {
    crypto: {
        enabled: false,
        maxDataLength: 240,
        includeData: true,
        includeBacktrace: false,
        algorithms: {
            aes: true,
            des: true,
            tripleDes: true,
            cast: true,
            rc4: true,
            rc2: true,
            blowfish: true
        }
    },
    hash: {
        enabled: false,
        maxInputDataLength: 240,
        includeData: true,
        includeBacktrace: false,
        algorithms: {
            md2: true,
            md4: true,
            md5: true,
            sha1: true,
            sha224: true,
            sha256: true,
            sha384: true,
            sha512: true
        }
    },
    hmac: {
        enabled: false,
        maxInputDataLength: 240,
        includeData: true,
        includeBacktrace: false
    },
    pbkdf: {
        enabled: false,
        includeBacktrace: false
    },
    keychain: {
        enabled: true,
        includeBacktrace: false,
        maxDataLength: 512,
        maxDepth: 5
    },
    seckey: {
        enabled: true,
        includeData: true,
        includeBacktrace: false,
        maxDataLength: 512,
        maxDepth: 5
    }
};
✄
{"version":3,"file":"commoncrypto.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/constants/commoncrypto.js"],"names":[],"mappings":"AAAA,MAAM,CAAC,MAAM,WAAW,GAAG;IACzB,CAAC,EAAE,YAAY;IACf,CAAC,EAAE,YAAY;IACf,CAAC,EAAE,SAAS;CACb,CAAC;AAEF,MAAM,CAAC,MAAM,WAAW,GAAG;IACzB,CAAC,EAAE,iBAAiB;IACpB,CAAC,EAAE,iBAAiB;IACpB,CAAC,EAAE,kBAAkB;IACrB,CAAC,EAAE,kBAAkB;IACrB,CAAC,EAAE,iBAAiB;IACpB,CAAC,EAAE,iBAAiB;IACpB,CAAC,EAAE,sBAAsB;CAC1B,CAAC;AAEF,MAAM,CAAC,MAAM,cAAc,GAAG;IAC5B,CAAC,EAAE,KAAK;IACR,CAAC,EAAE,KAAK;IACR,CAAC,EAAE,WAAW;IACd,CAAC,EAAE,MAAM;IACT,CAAC,EAAE,KAAK;IACR,CAAC,EAAE,KAAK;IACR,CAAC,EAAE,UAAU;CACd,CAAC;AAEF,MAAM,CAAC,MAAM,SAAS,GAAG;IACvB,CAAC,EAAE,MAAM;IACT,CAAC,EAAE,uBAAuB;IAC1B,CAAC,EAAE,kBAAkB;IACrB,CAAC,EAAE,wCAAwC;CAC5C,CAAC;AAEF,MAAM,CAAC,MAAM,MAAM,GAAG;IACpB,CAAC,EAAE,YAAY;IACf,CAAC,EAAE,YAAY;IACf,CAAC,EAAE,YAAY;IACf,CAAC,EAAE,YAAY;IACf,CAAC,EAAE,WAAW;IACd,CAAC,EAAE,YAAY;IACf,CAAC,EAAE,YAAY;IACf,CAAC,EAAE,YAAY;IACf,CAAC,EAAE,YAAY;IACf,EAAE,EAAE,aAAa;IACjB,EAAE,EAAE,YAAY;IAChB,EAAE,EAAE,YAAY;CACjB,CAAC;AAEF,MAAM,CAAC,MAAM,SAAS,GAAG;IACvB,CAAC,EAAE,aAAa;IAChB,CAAC,EAAE,gBAAgB;IACnB,EAAE,EAAE,WAAW;CAChB,CAAC;AAEF,MAAM,CAAC,MAAM,aAAa,GAAG;IAC3B,MAAM,EAAE,qBAAqB;IAC7B,MAAM,EAAE,qBAAqB;CAC9B,CAAC;AAEF,MAAM,CAAC,MAAM,eAAe,GAAG;IAC7B,CAAC,EAAE,gBAAgB;IACnB,CAAC,EAAE,eAAe;IAClB,CAAC,EAAE,kBAAkB;IACrB,CAAC,EAAE,kBAAkB;IACrB,CAAC,EAAE,kBAAkB;IACrB,CAAC,EAAE,kBAAkB;CACtB,CAAC;AAEF,MAAM,CAAC,MAAM,kBAAkB,GAAG;IAChC,CAAC,EAAE,EAAE;IACL,CAAC,EAAE,EAAE;IACL,CAAC,EAAE,EAAE;IACL,CAAC,EAAE,EAAE;IACL,CAAC,EAAE,EAAE;IACL,CAAC,EAAE,EAAE;CACN,CAAC;AAEF,MAAM,CAAC,MAAM,gBAAgB,GAAG;IAC9B,MAAM,EAAE,EAAE;IACV,MAAM,EAAE,EAAE;IACV,MAAM,EAAE,EAAE;IACV,OAAO,EAAE,EAAE;IACX,SAAS,EAAE,EAAE;IACb,SAAS,EAAE,EAAE;IACb,SAAS,EAAE,EAAE;IACb,SAAS,EAAE,EAAE;CACd,CAAC;AAEF,MAAM,CAAC,MAAM,uBAAuB,GAAG;IACrC,CAAC,EAAE,mBAAmB;IACtB,CAAC,EAAE,qBAAqB;IACxB,CAAC,EAAE,qBAAqB;IACxB,CAAC,EAAE,qBAAqB;IACxB,CAAC,EAAE,qBAAqB;CACzB,CAAC;AAEF,MAAM,CAAC,MAAM,gBAAgB,GAAG;IAC9B,CAAC,EAAE,WAAW;CACf,CAAC"}
✄
export const CCOperation = {
    0: "kCCEncrypt",
    1: "kCCDecrypt",
    3: "kCCBoth"
};
export const CCAlgorithm = {
    0: "kCCAlgorithmAES",
    1: "kCCAlgorithmDES",
    2: "kCCAlgorithm3DES",
    3: "kCCAlgorithmCAST",
    4: "kCCAlgorithmRC4",
    5: "kCCAlgorithmRC2",
    6: "kCCAlgorithmBlowfish"
};
export const CCAlgorithmKey = {
    0: "aes",
    1: "des",
    2: "tripleDes",
    3: "cast",
    4: "rc4",
    5: "rc2",
    6: "blowfish"
};
export const CCOptions = {
    0: "none",
    1: "kCCOptionPKCS7Padding",
    2: "kCCOptionECBMode",
    3: "kCCOptionPKCS7Padding|kCCOptionECBMode"
};
export const CCMode = {
    1: "kCCModeECB",
    2: "kCCModeCBC",
    3: "kCCModeCFB",
    4: "kCCModeCTR",
    5: "kCCModeF8",
    6: "kCCModeLRW",
    7: "kCCModeOFB",
    8: "kCCModeXTS",
    9: "kCCModeRC4",
    10: "kCCModeCFB8",
    11: "kCCModeGCM",
    12: "kCCModeCCM"
};
export const CCPadding = {
    0: "ccNoPadding",
    1: "ccPKCS7Padding",
    12: "ccCBCCTS3"
};
export const CCModeOptions = {
    0x0001: "kCCModeOptionCTR_LE",
    0x0002: "kCCModeOptionCTR_BE"
};
export const CCHmacAlgorithm = {
    0: "kCCHmacAlgSHA1",
    1: "kCCHmacAlgMD5",
    2: "kCCHmacAlgSHA256",
    3: "kCCHmacAlgSHA384",
    4: "kCCHmacAlgSHA512",
    5: "kCCHmacAlgSHA224"
};
export const CCHmacDigestLength = {
    0: 20,
    1: 16,
    2: 32,
    3: 48,
    4: 64,
    5: 28
};
export const HashDigestLength = {
    CC_MD2: 16,
    CC_MD4: 16,
    CC_MD5: 16,
    CC_SHA1: 20,
    CC_SHA224: 28,
    CC_SHA256: 32,
    CC_SHA384: 48,
    CC_SHA512: 64
};
export const CCPseudoRandomAlgorithm = {
    1: "kCCPRFHmacAlgSHA1",
    2: "kCCPRFHmacAlgSHA224",
    3: "kCCPRFHmacAlgSHA256",
    4: "kCCPRFHmacAlgSHA384",
    5: "kCCPRFHmacAlgSHA512"
};
export const CCPBKDFAlgorithm = {
    2: "kCCPBKDF2"
};
✄
{"version":3,"file":"ios.cipher.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/modules/ios.cipher.js"],"names":[],"mappings":"AAAA,OAAO,EAAE,iBAAiB,EAAE,MAAM,cAAc,CAAC;AACjD,OAAO,EAAE,qBAAqB,EAAE,MAAM,iBAAiB,CAAC;AACxD,OAAO,EAAE,mBAAmB,EAAE,MAAM,eAAe,CAAC;AACpD,OAAO,EAAE,mBAAmB,EAAE,MAAM,eAAe,CAAC;AACpD,OAAO,EAAE,oBAAoB,EAAE,MAAM,gBAAgB,CAAC;AACtD,OAAO,EAAE,uBAAuB,EAAE,MAAM,mBAAmB,CAAC;AAC5D,OAAO,EAAE,qBAAqB,EAAE,MAAM,iBAAiB,CAAC;AAExD,MAAM,UAAU,qBAAqB,CAAC,EAAE,IAAI,EAAE,MAAM,GAAG,iBAAiB,EAAE;IACxE,gBAAgB,CAAC,yBAAyB,EAAE,GAAG,EAAE,CAAC,qBAAqB,CAAC,EAAE,IAAI,EAAE,MAAM,EAAE,MAAM,CAAC,MAAM,EAAE,CAAC,EAAE,IAAI,CAAC,CAAC;IAChH,gBAAgB,CAAC,iBAAiB,EAAE,GAAG,EAAE,CAAC,mBAAmB,CAAC,EAAE,IAAI,EAAE,MAAM,EAAE,MAAM,CAAC,IAAI,EAAE,CAAC,EAAE,IAAI,CAAC,CAAC;IACpG,gBAAgB,CAAC,iBAAiB,EAAE,GAAG,EAAE,CAAC,mBAAmB,CAAC,EAAE,IAAI,EAAE,MAAM,EAAE,MAAM,CAAC,IAAI,EAAE,CAAC,EAAE,IAAI,CAAC,CAAC;IACpG,gBAAgB,CAAC,kBAAkB,EAAE,GAAG,EAAE,CAAC,oBAAoB,CAAC,EAAE,IAAI,EAAE,MAAM,EAAE,MAAM,CAAC,KAAK,EAAE,CAAC,EAAE,IAAI,CAAC,CAAC;IACvG,gBAAgB,CAAC,qBAAqB,EAAE,GAAG,EAAE,CAAC,uBAAuB,CAAC,EAAE,IAAI,EAAE,MAAM,EAAE,MAAM,CAAC,QAAQ,EAAE,CAAC,EAAE,IAAI,CAAC,CAAC;IAChH,gBAAgB,CAAC,mBAAmB,EAAE,GAAG,EAAE,CAAC,qBAAqB,CAAC,EAAE,IAAI,EAAE,MAAM,EAAE,MAAM,CAAC,MAAM,EAAE,CAAC,EAAE,IAAI,CAAC,CAAC;AAC5G,CAAC;AAED,SAAS,gBAAgB,CAAC,WAAW,EAAE,SAAS,EAAE,IAAI;IACpD,IAAI;QACF,IAAI,CAAC,EAAE,IAAI,EAAE,sBAAsB,EAAE,WAAW,EAAE,CAAC,CAAC;QACpD,SAAS,EAAE,CAAC;QACZ,IAAI,CAAC,EAAE,IAAI,EAAE,qBAAqB,EAAE,WAAW,EAAE,CAAC,CAAC;KACpD;IAAC,OAAO,CAAC,EAAE;QACV,IAAI,CAAC;YACH,IAAI,EAAE,iBAAiB;YACvB,WAAW;YACX,OAAO,EAAE,MAAM,CAAC,CAAC,CAAC;YAClB,KAAK,EAAE,CAAC,IAAI,CAAC,CAAC,KAAK,CAAC,CAAC,CAAC,MAAM,CAAC,CAAC,CAAC,KAAK,CAAC,CAAC,CAAC,CAAC,IAAI;SAC7C,CAAC,CAAC;KACJ;AACH,CAAC"}
✄
import { IOS_CIPHER_CONFIG } from "../config.js";
import { installIosCryptoHooks } from "./ios.crypto.js";
import { installIosHashHooks } from "./ios.hash.js";
import { installIosHmacHooks } from "./ios.hmac.js";
import { installIosPbkdfHooks } from "./ios.pbkdf.js";
import { installIosKeychainHooks } from "./ios.keychain.js";
import { installIosSecKeyHooks } from "./ios.seckey.js";
export function installIosCipherHooks({ emit, config = IOS_CIPHER_CONFIG }) {
    installSubmodule("ios.cipher.commoncrypto", () => installIosCryptoHooks({ emit, config: config.crypto }), emit);
    installSubmodule("ios.cipher.hash", () => installIosHashHooks({ emit, config: config.hash }), emit);
    installSubmodule("ios.cipher.hmac", () => installIosHmacHooks({ emit, config: config.hmac }), emit);
    installSubmodule("ios.cipher.pbkdf", () => installIosPbkdfHooks({ emit, config: config.pbkdf }), emit);
    installSubmodule("ios.cipher.keychain", () => installIosKeychainHooks({ emit, config: config.keychain }), emit);
    installSubmodule("ios.cipher.seckey", () => installIosSecKeyHooks({ emit, config: config.seckey }), emit);
}
function installSubmodule(submoduleId, installer, emit) {
    try {
        emit({ type: "submodule.installing", submoduleId });
        installer();
        emit({ type: "submodule.installed", submoduleId });
    }
    catch (e) {
        emit({
            type: "submodule.error",
            submoduleId,
            message: String(e),
            stack: e && e.stack ? String(e.stack) : null
        });
    }
}
✄
{"version":3,"file":"ios.crypto.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/modules/ios.crypto.js"],"names":[],"mappings":"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"}
✄
import { IOS_CIPHER_CONFIG } from "../config.js";
import { CCAlgorithm, CCAlgorithmKey, CCMode, CCModeOptions, CCOperation, CCOptions, CCPadding } from "../constants/commoncrypto.js";
import { attachExport, isNull, toInt, toSize, toUInt } from "../utils/native.js";
import { readByteArraySafe, readSizeTPtr } from "../utils/data.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { enumName, optionNames } from "../utils/format.js";
const cryptorState = new Map();
let nextSessionId = 1;
export function installIosCryptoHooks({ emit, config = IOS_CIPHER_CONFIG.crypto }) {
    if (!config.enabled)
        return;
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
    if (operation === 0)
        return "encrypt";
    if (operation === 1)
        return "decrypt";
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
            if (this.skip)
                return;
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
            if (this.skip)
                return;
            const cryptorRef = readPointerSafe(this.cryptorRefPtr);
            if (cryptorRef !== null)
                cryptorState.set(cryptorRef.toString(), this.state);
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
            if (this.skip)
                return;
            const cryptorRef = readPointerSafe(this.cryptorRefPtr);
            if (cryptorRef !== null)
                cryptorState.set(cryptorRef.toString(), this.state);
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
            if (!isNull(this.cryptor))
                cryptorState.delete(this.cryptor.toString());
        }
    }, emit);
}
function blockIvGuessLength(algorithm) {
    if (algorithm === 0)
        return 16;
    if (algorithm === 2)
        return 8;
    if (algorithm === 1 || algorithm === 3 || algorithm === 5 || algorithm === 6)
        return 8;
    return 0;
}
function readPointerSafe(address) {
    if (isNull(address))
        return null;
    try {
        const p = address.readPointer();
        return p.isNull() ? null : p;
    }
    catch (_) {
        return null;
    }
}
✄
{"version":3,"file":"ios.hash.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/modules/ios.hash.js"],"names":[],"mappings":"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"}
✄
import { IOS_CIPHER_CONFIG } from "../config.js";
import { HashDigestLength } from "../constants/commoncrypto.js";
import { attachExport, isNull, toSize } from "../utils/native.js";
import { readByteArraySafe } from "../utils/data.js";
import { captureBacktrace } from "../utils/backtrace.js";
const HASHES = ["CC_MD2", "CC_MD4", "CC_MD5", "CC_SHA1", "CC_SHA224", "CC_SHA256", "CC_SHA384", "CC_SHA512"];
const hashContexts = new Map();
export function installIosHashHooks({ emit, config = IOS_CIPHER_CONFIG.hash }) {
    if (!config.enabled)
        return;
    HASHES.forEach(name => {
        const configKey = name.replace("CC_", "").toLowerCase();
        if (config.algorithms[configKey] === false)
            return;
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
            if (!isNull(ctx))
                hashContexts.set(ctx.toString(), { algorithm: name, totalInputLength: 0 });
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
            if (!isNull(this.ctx))
                hashContexts.delete(this.ctx.toString());
        }
    }, emit);
}
function dataField(address, length, config) {
    return config.includeData ? readByteArraySafe(address, length, config.maxInputDataLength) : { length };
}
✄
{"version":3,"file":"ios.hmac.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/modules/ios.hmac.js"],"names":[],"mappings":"AAAA,OAAO,EAAE,iBAAiB,EAAE,MAAM,cAAc,CAAC;AACjD,OAAO,EAAE,eAAe,EAAE,kBAAkB,EAAE,MAAM,8BAA8B,CAAC;AACnF,OAAO,EAAE,YAAY,EAAE,MAAM,EAAE,KAAK,EAAE,MAAM,EAAE,MAAM,oBAAoB,CAAC;AACzE,OAAO,EAAE,iBAAiB,EAAE,MAAM,kBAAkB,CAAC;AACrD,OAAO,EAAE,gBAAgB,EAAE,MAAM,uBAAuB,CAAC;AACzD,OAAO,EAAE,QAAQ,EAAE,MAAM,oBAAoB,CAAC;AAE9C,MAAM,YAAY,GAAG,IAAI,GAAG,EAAE,CAAC;AAE/B,MAAM,UAAU,mBAAmB,CAAC,EAAE,IAAI,EAAE,MAAM,GAAG,iBAAiB,CAAC,IAAI,EAAE;IAC3E,IAAI,CAAC,MAAM,CAAC,OAAO;QAAE,OAAO;IAC5B,UAAU,CAAC,IAAI,EAAE,MAAM,CAAC,CAAC;IACzB,cAAc,CAAC,IAAI,EAAE,MAAM,CAAC,CAAC;IAC7B,gBAAgB,CAAC,IAAI,EAAE,MAAM,CAAC,CAAC;IAC/B,eAAe,CAAC,IAAI,EAAE,MAAM,CAAC,CAAC;AAChC,CAAC;AAED,SAAS,UAAU,CAAC,IAAI,EAAE,MAAM;IAC9B,YAAY,CAAC,QAAQ,EAAE;QACrB,OAAO,CAAC,IAAI;YACV,IAAI,CAAC,SAAS,GAAG,KAAK,CAAC,IAAI,CAAC,CAAC,CAAC,CAAC,CAAC;YAChC,IAAI,CAAC,YAAY,GAAG,kBAAkB,CAAC,IAAI,CAAC,SAAS,CAAC,IAAI,CAAC,CAAC;YAC5D,IAAI,CAAC,OAAO,GAAG,IAAI,CAAC,CAAC,CAAC,CAAC;YACvB,IAAI,CAAC,KAAK,GAAG;gBACX,IAAI,EAAE,iBAAiB;gBACvB,QAAQ,EAAE,QAAQ;gBAClB,KAAK,EAAE,2BAA2B;gBAClC,GAAG,EAAE,QAAQ;gBACb,SAAS,EAAE,MAAM;gBACjB,SAAS,EAAE,QAAQ,CAAC,eAAe,EAAE,IAAI,CAAC,SAAS,CAAC;gBACpD,GAAG,EAAE,SAAS,CAAC,IAAI,CAAC,CAAC,CAAC,EAAE,MAAM,CAAC,IAAI,CAAC,CAAC,CAAC,CAAC,EAAE,MAAM,CAAC;gBAChD,KAAK,EAAE,SAAS,CAAC,IAAI,CAAC,CAAC,CAAC,EAAE,MAAM,CAAC,IAAI,CAAC,CAAC,CAAC,CAAC,EAAE,MAAM,CAAC;gBAClD,aAAa,EAAE,MAAM,CAAC,gBAAgB,CAAC,CAAC,CAAC,QAAQ,CAAC,CAAC,CAAC,IAAI;gBACxD,SAAS,EAAE,gBAAgB,CAAC,IAAI,CAAC,OAAO,EAAE,MAAM,CAAC,gBAAgB,CAAC;aACnE,CAAC;QACJ,CAAC;QACD,OAAO;YACL,IAAI,CAAC;gBACH,GAAG,IAAI,CAAC,KAAK;gBACb,MAAM,EAAE,SAAS,CAAC,IAAI,CAAC,OAAO,EAAE,IAAI,CAAC,YAAY,EAAE,MAAM,CAAC;aAC3D,CAAC,CAAC;QACL,CAAC;KACF,EAAE,IAAI,CAAC,CAAC;AACX,CAAC;AAED,SAAS,cAAc,CAAC,IAAI,EAAE,MAAM;IAClC,YAAY,CAAC,YAAY,EAAE;QACzB,OAAO,CAAC,IAAI;YACV,MAAM,GAAG,GAAG,IAAI,CAAC,CAAC,CAAC,CAAC;YACpB,MAAM,SAAS,GAAG,KAAK,CAAC,IAAI,CAAC,CAAC,CAAC,CAAC,CAAC;YACjC,MAAM,KAAK,GAAG;gBACZ,SAAS,EAAE,QAAQ,CAAC,eAAe,EAAE,SAAS,CAAC;gBAC/C,gBAAgB,EAAE,CAAC;gBACnB,GAAG,EAAE,SAAS,CAAC,IAAI,CAAC,CAAC,CAAC,EAAE,MAAM,CAAC,IAAI,CAAC,CAAC,CAAC,CAAC,EAAE,MAAM,CAAC;aACjD,CAAC;YACF,IAAI,CAAC,MAAM,CAAC,GAAG,CAAC;gBAAE,YAAY,CAAC,GAAG,CAAC,GAAG,CAAC,QAAQ,EAAE,EAAE,KAAK,CAAC,CAAC;YAC1D,IAAI,CAAC;gBACH,IAAI,EAAE,iBAAiB;gBACvB,QAAQ,EAAE,QAAQ;gBAClB,KAAK,EAAE,wBAAwB;gBAC/B,GAAG,EAAE,YAAY;gBACjB,OAAO,EAAE,GAAG,CAAC,QAAQ,EAAE;gBACvB,GAAG,KAAK;gBACR,aAAa,EAAE,MAAM,CAAC,gBAAgB,CAAC,CAAC,CAAC,QAAQ,CAAC,CAAC,CAAC,IAAI;gBACxD,SAAS,EAAE,gBAAgB,CAAC,IAAI,CAAC,OAAO,EAAE,MAAM,CAAC,gBAAgB,CAAC;aACnE,CAAC,CAAC;QACL,CAAC;KACF,EAAE,IAAI,CAAC,CAAC;AACX,CAAC;AAED,SAAS,gBAAgB,CAAC,IAAI,EAAE,MAAM;IACpC,YAAY,CAAC,cAAc,EAAE;QAC3B,OAAO,CAAC,IAAI;YACV,MAAM,GAAG,GAAG,IAAI,CAAC,CAAC,CAAC,CAAC;YACpB,MAAM,MAAM,GAAG,MAAM,CAAC,IAAI,CAAC,CAAC,CAAC,CAAC,CAAC;YAC/B,MAAM,KAAK,GAAG,CAAC,MAAM,CAAC,GAAG,CAAC,CAAC,CAAC,CAAC,YAAY,CAAC,GAAG,CAAC,GAAG,CAAC,QAAQ,EAAE,CAAC,IAAI,IAAI,CAAC,CAAC,CAAC,IAAI,CAAC;YAC7E,IAAI,KAAK,KAAK,IAAI,EAAE;gBAClB,KAAK,CAAC,gBAAgB,IAAI,MAAM,CAAC;gBACjC,YAAY,CAAC,GAAG,CAAC,GAAG,CAAC,QAAQ,EAAE,EAAE,KAAK,CAAC,CAAC;aACzC;YACD,IAAI,CAAC;gBACH,IAAI,EAAE,iBAAiB;gBACvB,QAAQ,EAAE,QAAQ;gBAClB,KAAK,EAAE,0BAA0B;gBACjC,GAAG,EAAE,cAAc;gBACnB,OAAO,EAAE,GAAG,CAAC,QAAQ,EAAE;gBACvB,SAAS,EAAE,KAAK;gBAChB,KAAK,EAAE,SAAS,CAAC,IAAI,CAAC,CAAC,CAAC,EAAE,MAAM,EAAE,MAAM,CAAC;gBACzC,aAAa,EAAE,MAAM,CAAC,gBAAgB,CAAC,CAAC,CAAC,QAAQ,CAAC,CAAC,CAAC,IAAI;gBACxD,SAAS,EAAE,gBAAgB,CAAC,IAAI,CAAC,OAAO,EAAE,MAAM,CAAC,gBAAgB,CAAC;aACnE,CAAC,CAAC;QACL,CAAC;KACF,EAAE,IAAI,CAAC,CAAC;AACX,CAAC;AAED,SAAS,eAAe,CAAC,IAAI,EAAE,MAAM;IACnC,YAAY,CAAC,aAAa,EAAE;QAC1B,OAAO,CAAC,IAAI;YACV,IAAI,CAAC,GAAG,GAAG,IAAI,CAAC,CAAC,CAAC,CAAC;YACnB,IAAI,CAAC,MAAM,GAAG,IAAI,CAAC,CAAC,CAAC,CAAC;YACtB,IAAI,CAAC,KAAK,GAAG,CAAC,MAAM,CAAC,IAAI,CAAC,GAAG,CAAC,CAAC,CAAC,CAAC,YAAY,CAAC,GAAG,CAAC,IAAI,CAAC,GAAG,CAAC,QAAQ,EAAE,CAAC,IAAI,IAAI,CAAC,CAAC,CAAC,IAAI,CAAC;QACxF,CAAC;QACD,OAAO;YACL,IAAI,YAAY,GAAG,CAAC,CAAC;YACrB,IAAI,IAAI,CAAC,KAAK,KAAK,IAAI,EAAE;gBACvB,MAAM,OAAO,GAAG,IAAI,CAAC,KAAK,CAAC,SAAS,CAAC;gBACrC,MAAM,CAAC,IAAI,CAAC,eAAe,CAAC,CAAC,OAAO,CAAC,CAAC,CAAC,EAAE;oBACvC,IAAI,eAAe,CAAC,CAAC,CAAC,KAAK,OAAO;wBAAE,YAAY,GAAG,kBAAkB,CAAC,CAAC,CAAC,IAAI,CAAC,CAAC;gBAChF,CAAC,CAAC,CAAC;aACJ;YACD,IAAI,CAAC;gBACH,IAAI,EAAE,iBAAiB;gBACvB,QAAQ,EAAE,QAAQ;gBAClB,KAAK,EAAE,yBAAyB;gBAChC,GAAG,EAAE,aAAa;gBAClB,OAAO,EAAE,IAAI,CAAC,GAAG,CAAC,QAAQ,EAAE;gBAC5B,SAAS,EAAE,IAAI,CAAC,KAAK;gBACrB,MAAM,EAAE,SAAS,CAAC,IAAI,CAAC,MAAM,EAAE,YAAY,EAAE,MAAM,CAAC;gBACpD,aAAa,EAAE,MAAM,CAAC,gBAAgB,CAAC,CAAC,CAAC,QAAQ,CAAC,CAAC,CAAC,IAAI;gBACxD,SAAS,EAAE,gBAAgB,CAAC,IAAI,CAAC,OAAO,EAAE,MAAM,CAAC,gBAAgB,CAAC;aACnE,CAAC,CAAC;YACH,IAAI,CAAC,MAAM,CAAC,IAAI,CAAC,GAAG,CAAC;gBAAE,YAAY,CAAC,MAAM,CAAC,IAAI,CAAC,GAAG,CAAC,QAAQ,EAAE,CAAC,CAAC;QAClE,CAAC;KACF,EAAE,IAAI,CAAC,CAAC;AACX,CAAC;AAED,SAAS,SAAS,CAAC,OAAO,EAAE,MAAM,EAAE,MAAM;IACxC,OAAO,MAAM,CAAC,WAAW,CAAC,CAAC,CAAC,iBAAiB,CAAC,OAAO,EAAE,MAAM,EAAE,MAAM,CAAC,kBAAkB,CAAC,CAAC,CAAC,CAAC,EAAE,MAAM,EAAE,CAAC;AACzG,CAAC"}
✄
import { IOS_CIPHER_CONFIG } from "../config.js";
import { CCHmacAlgorithm, CCHmacDigestLength } from "../constants/commoncrypto.js";
import { attachExport, isNull, toInt, toSize } from "../utils/native.js";
import { readByteArraySafe } from "../utils/data.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { enumName } from "../utils/format.js";
const hmacContexts = new Map();
export function installIosHmacHooks({ emit, config = IOS_CIPHER_CONFIG.hmac }) {
    if (!config.enabled)
        return;
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
            if (!isNull(ctx))
                hmacContexts.set(ctx.toString(), state);
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
                    if (CCHmacAlgorithm[k] === algName)
                        digestLength = CCHmacDigestLength[k] || 0;
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
            if (!isNull(this.ctx))
                hmacContexts.delete(this.ctx.toString());
        }
    }, emit);
}
function dataField(address, length, config) {
    return config.includeData ? readByteArraySafe(address, length, config.maxInputDataLength) : { length };
}
✄
{"version":3,"file":"ios.keychain.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/modules/ios.keychain.js"],"names":[],"mappings":"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"}
✄
import { IOS_CIPHER_CONFIG } from "../config.js";
import { attachExport, isNull, toInt } from "../utils/native.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { parseCfType } from "../utils/cf.js";
export function installIosKeychainHooks({ emit, config = IOS_CIPHER_CONFIG.keychain }) {
    if (!config.enabled)
        return;
    hookSecItemAdd(emit, config);
    hookSecItemCopyMatching(emit, config);
    hookSecItemUpdate(emit, config);
    hookSecItemDelete(emit, config);
}
function hookSecItemAdd(emit, config) {
    attachExport("SecItemAdd", {
        onEnter(args) {
            this.query = args[0];
            this.resultPtr = args[1];
            this.event = baseKeychainEvent("keychain.add", "SecItemAdd", "add", this.context, config);
            this.item = parseKeychainDictionary(this.query, config);
        },
        onLeave(retval) {
            emit({
                ...this.event,
                item: this.item,
                result: readOutCfType(this.resultPtr, config),
                status: toInt(retval)
            });
        }
    }, emit);
}
function hookSecItemCopyMatching(emit, config) {
    attachExport("SecItemCopyMatching", {
        onEnter(args) {
            this.query = args[0];
            this.resultPtr = args[1];
            this.event = baseKeychainEvent("keychain.copy_matching", "SecItemCopyMatching", "copy_matching", this.context, config);
            this.queryObject = parseKeychainDictionary(this.query, config);
        },
        onLeave(retval) {
            emit({
                ...this.event,
                query: this.queryObject,
                result: readOutCfType(this.resultPtr, config),
                status: toInt(retval)
            });
        }
    }, emit);
}
function hookSecItemUpdate(emit, config) {
    attachExport("SecItemUpdate", {
        onEnter(args) {
            this.query = args[0];
            this.attributesToUpdate = args[1];
            this.event = baseKeychainEvent("keychain.update", "SecItemUpdate", "update", this.context, config);
            this.queryObject = parseKeychainDictionary(this.query, config);
            this.updateObject = parseKeychainDictionary(this.attributesToUpdate, config);
        },
        onLeave(retval) {
            emit({
                ...this.event,
                query: this.queryObject,
                attributesToUpdate: this.updateObject,
                status: toInt(retval)
            });
        }
    }, emit);
}
function hookSecItemDelete(emit, config) {
    attachExport("SecItemDelete", {
        onEnter(args) {
            this.query = args[0];
            this.event = baseKeychainEvent("keychain.delete", "SecItemDelete", "delete", this.context, config);
            this.queryObject = parseKeychainDictionary(this.query, config);
        },
        onLeave(retval) {
            emit({
                ...this.event,
                query: this.queryObject,
                status: toInt(retval)
            });
        }
    }, emit);
}
function baseKeychainEvent(event, api, operation, context, config) {
    return {
        type: "telemetry.event",
        category: "storage",
        event,
        api,
        operation,
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(context, config.includeBacktrace)
    };
}
function parseKeychainDictionary(value, config) {
    const parsed = parseCfType(value, {
        maxDepth: config.maxDepth || 5,
        maxDataLength: config.maxDataLength || 512
    });
    if (parsed === null)
        return null;
    return parsed.type === "dictionary" ? parsed.value : parsed;
}
function readOutCfType(resultPtr, config) {
    if (resultPtr === null || resultPtr === undefined || isNull(resultPtr))
        return null;
    try {
        const value = resultPtr.readPointer();
        if (value.isNull())
            return null;
        const parsed = parseCfType(value, {
            maxDepth: config.maxDepth || 5,
            maxDataLength: config.maxDataLength || 512
        });
        if (parsed === null)
            return null;
        if (parsed.type === "dictionary" || parsed.type === "array" || parsed.type === "data")
            return parsed.value;
        if (parsed.type === "string" || parsed.type === "boolean" || parsed.type === "number")
            return parsed.value;
        return parsed;
    }
    catch (e) {
        return { error: String(e), pointer: resultPtr.toString() };
    }
}
✄
{"version":3,"file":"ios.pbkdf.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/modules/ios.pbkdf.js"],"names":[],"mappings":"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"}
✄
import { IOS_CIPHER_CONFIG } from "../config.js";
import { CCPBKDFAlgorithm, CCPseudoRandomAlgorithm } from "../constants/commoncrypto.js";
import { attachExport, toInt, toSize, toUInt } from "../utils/native.js";
import { readByteArraySafe, readUtf8StringSafe } from "../utils/data.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { enumName } from "../utils/format.js";
export function installIosPbkdfHooks({ emit, config = IOS_CIPHER_CONFIG.pbkdf }) {
    if (!config.enabled)
        return;
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
✄
{"version":3,"file":"ios.seckey.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/modules/ios.seckey.js"],"names":[],"mappings":"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"}
✄
import { IOS_CIPHER_CONFIG } from "../config.js";
import { attachExport, isNull } from "../utils/native.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { parseCfType } from "../utils/cf.js";
export function installIosSecKeyHooks({ emit, config = IOS_CIPHER_CONFIG.seckey }) {
    if (!config.enabled)
        return;
    hookSecKeyCreateSignature(emit, config);
}
function hookSecKeyCreateSignature(emit, config) {
    attachExport("SecKeyCreateSignature", {
        onEnter(args) {
            this.key = args[0];
            this.algorithm = args[1];
            this.dataToSign = args[2];
            this.errorPtr = args[3];
            this.event = {
                type: "telemetry.event",
                category: "crypto",
                event: "seckey.create_signature",
                api: "SecKeyCreateSignature",
                operation: "sign",
                key: parseSecKey(this.key, config),
                algorithm: parseCfValue(this.algorithm, config),
                dataToSign: config.includeData
                    ? parseCfValue(this.dataToSign, config)
                    : parseCfDataMetadata(this.dataToSign, config),
                backtraceType: config.includeBacktrace ? "native" : null,
                backtrace: captureBacktrace(this.context, config.includeBacktrace)
            };
        },
        onLeave(retval) {
            emit({
                ...this.event,
                signature: config.includeData
                    ? parseCfValue(retval, config)
                    : parseCfDataMetadata(retval, config),
                error: readOutCfError(this.errorPtr, config),
                success: !isNull(retval)
            });
        }
    }, emit);
}
function parseSecKey(value, config) {
    if (value === null || value === undefined || isNull(value)) {
        return null;
    }
    const parsed = parseCfType(value, {
        maxDepth: config.maxDepth || 5,
        maxDataLength: config.maxDataLength || 512
    });
    if (parsed === null) {
        return {
            pointer: value.toString()
        };
    }
    return {
        pointer: value.toString(),
        value: parsed
    };
}
function parseCfValue(value, config) {
    if (value === null || value === undefined || isNull(value)) {
        return null;
    }
    return parseCfType(value, {
        maxDepth: config.maxDepth || 5,
        maxDataLength: config.maxDataLength || 512
    });
}
function parseCfDataMetadata(value, config) {
    const parsed = parseCfValue(value, config);
    if (parsed === null) {
        return null;
    }
    if (parsed.type === "data" && parsed.value) {
        return {
            type: "data",
            length: parsed.value.length,
            capturedLength: parsed.value.capturedLength,
            truncated: parsed.value.truncated
        };
    }
    return parsed;
}
function readOutCfError(errorPtr, config) {
    if (errorPtr === null || errorPtr === undefined || isNull(errorPtr)) {
        return null;
    }
    try {
        const error = errorPtr.readPointer();
        if (error.isNull()) {
            return null;
        }
        return parseCfType(error, {
            maxDepth: config.maxDepth || 5,
            maxDataLength: config.maxDataLength || 512
        });
    }
    catch (e) {
        return {
            error: String(e),
            pointer: errorPtr.toString()
        };
    }
}
✄
{"version":3,"file":"backtrace.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/utils/backtrace.js"],"names":[],"mappings":"AAAA,MAAM,UAAU,gBAAgB,CAAC,OAAO,EAAE,OAAO;IAC/C,IAAI,CAAC,OAAO;QAAE,OAAO,IAAI,CAAC;IAC1B,IAAI;QACF,OAAO,MAAM,CAAC,SAAS,CAAC,OAAO,EAAE,UAAU,CAAC,QAAQ,CAAC;aAClD,GAAG,CAAC,OAAO,CAAC,EAAE,CAAC,WAAW,CAAC,WAAW,CAAC,OAAO,CAAC,CAAC,QAAQ,EAAE,CAAC,CAAC;KAChE;IAAC,OAAO,CAAC,EAAE;QACV,OAAO,CAAC,oBAAoB,GAAG,MAAM,CAAC,CAAC,CAAC,GAAG,GAAG,CAAC,CAAC;KACjD;AACH,CAAC"}
✄
export function captureBacktrace(context, enabled) {
    if (!enabled)
        return null;
    try {
        return Thread.backtrace(context, Backtracer.ACCURATE)
            .map(address => DebugSymbol.fromAddress(address).toString());
    }
    catch (e) {
        return ["<backtrace error: " + String(e) + ">"];
    }
}
✄
{"version":3,"file":"cf.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/utils/cf.js"],"names":[],"mappings":"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"}
✄
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
    if (initialized)
        return;
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
    if (p !== null)
        api[name] = new NativeFunction(p, ret, args);
}
function has(...names) {
    for (let i = 0; i < names.length; i++)
        if (!api[names[i]])
            return false;
    return true;
}
function sameType(value, typeFnName) {
    try {
        return has("CFGetTypeID", typeFnName) && Number(api.CFGetTypeID(value)) === Number(api[typeFnName]());
    }
    catch (_) {
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
    if (value === null || value === undefined || isNull(value))
        return null;
    const pointer = value.toString();
    if (ctx.seen[pointer])
        return { type: "reference", pointer };
    if (ctx.depth > ctx.maxDepth)
        return { type: "max-depth", pointer, description: copyCfDescription(value) };
    ctx.seen[pointer] = true;
    try {
        if (sameType(value, "CFStringGetTypeID"))
            return parseString(value);
        if (sameType(value, "CFDataGetTypeID"))
            return parseData(value, ctx.maxDataLength);
        if (sameType(value, "CFDictionaryGetTypeID"))
            return parseDictionary(value, childCtx(ctx));
        if (sameType(value, "CFArrayGetTypeID"))
            return parseArray(value, childCtx(ctx));
        if (sameType(value, "CFBooleanGetTypeID"))
            return parseBoolean(value);
        if (sameType(value, "CFNumberGetTypeID"))
            return parseNumber(value);
        if (sameType(value, "CFNullGetTypeID"))
            return null;
        return { type: "unknown", pointer, description: copyCfDescription(value) };
    }
    catch (e) {
        return { type: "error", pointer, error: String(e), description: copyCfDescription(value) };
    }
    finally {
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
    if (!has("CFDataGetLength", "CFDataGetBytePtr"))
        return { type: "data", pointer: value.toString(), description: copyCfDescription(value) };
    const length = Number(api.CFDataGetLength(value));
    const bytes = api.CFDataGetBytePtr(value);
    return {
        type: "data",
        value: readByteArraySafe(bytes, length, maxDataLength)
    };
}
function parseDictionary(value, ctx) {
    if (!has("CFDictionaryGetCount", "CFDictionaryGetKeysAndValues"))
        return { type: "dictionary", pointer: value.toString(), description: copyCfDescription(value) };
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
    if (!has("CFArrayGetCount", "CFArrayGetValueAtIndex"))
        return { type: "array", pointer: value.toString(), description: copyCfDescription(value) };
    const count = Number(api.CFArrayGetCount(value));
    const out = [];
    for (let i = 0; i < count; i++) {
        out.push(unwrapKeychainValue(parseAny(api.CFArrayGetValueAtIndex(value, i), ctx)));
    }
    return { type: "array", count, value: out };
}
function parseBoolean(value) {
    if (!has("CFBooleanGetValue"))
        return { type: "boolean", pointer: value.toString(), description: copyCfDescription(value) };
    return { type: "boolean", value: !!api.CFBooleanGetValue(value) };
}
function parseNumber(value) {
    if (!has("CFNumberGetValue"))
        return { type: "number", pointer: value.toString(), description: copyCfDescription(value) };
    const out = Memory.alloc(8);
    const ok = api.CFNumberGetValue(value, kCFNumberSInt64Type, out);
    return ok ? { type: "number", value: Number(out.readS64()) } : { type: "number", pointer: value.toString(), description: copyCfDescription(value) };
}
function cfStringToJsString(value) {
    if (!has("CFStringGetCString"))
        return String(copyCfDescription(value));
    const capacity = 8192;
    const buffer = Memory.alloc(capacity);
    const ok = api.CFStringGetCString(value, buffer, capacity, kCFStringEncodingUTF8);
    return ok ? readCStringSafe(buffer) : String(copyCfDescription(value));
}
function unwrapKeychainValue(parsed) {
    if (parsed === null)
        return null;
    if (parsed.type === "string")
        return parsed.value;
    if (parsed.type === "boolean")
        return parsed.value;
    if (parsed.type === "number")
        return parsed.value;
    if (parsed.type === "data")
        return parsed.value;
    if (parsed.type === "array")
        return parsed.value;
    if (parsed.type === "dictionary")
        return parsed.value;
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
        if (desc.isNull())
            return null;
        return cfStringToJsString(desc);
    }
    catch (e) {
        return { error: String(e), pointer: value.toString() };
    }
    finally {
        if (desc !== NULL && !desc.isNull() && api.CFRelease) {
            try {
                api.CFRelease(desc);
            }
            catch (_) { }
        }
    }
}
✄
{"version":3,"file":"data.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/utils/data.js"],"names":[],"mappings":"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"}
✄
import { isNull } from "./native.js";
const HEX = "0123456789abcdef";
export function readByteArraySafe(address, length, maxLength) {
    if (address === null || address === undefined || isNull(address))
        return null;
    const requestedLength = Math.max(0, Number(length) || 0);
    const effectiveLength = Math.min(requestedLength, maxLength || requestedLength);
    if (effectiveLength === 0) {
        return { length: requestedLength, capturedLength: 0, truncated: requestedLength > 0, hex: "", utf8: "" };
    }
    try {
        const bytes = address.readByteArray(effectiveLength);
        if (bytes === null)
            return null;
        return binaryField(bytes, requestedLength, effectiveLength);
    }
    catch (e) {
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
    if (address === null || address === undefined || isNull(address))
        return null;
    try {
        if (maxLength !== undefined && maxLength !== null && maxLength >= 0) {
            const bytes = address.readByteArray(maxLength);
            return bytes === null ? null : arrayBufferToUtf8Preview(bytes);
        }
        return address.readUtf8String();
    }
    catch (e) {
        return { error: String(e), pointer: address.toString() };
    }
}
export function readCStringSafe(address) {
    if (address === null || address === undefined || isNull(address))
        return null;
    try {
        return address.readUtf8String();
    }
    catch (e) {
        return { error: String(e), pointer: address.toString() };
    }
}
export function readSizeTPtr(address) {
    if (address === null || address === undefined || isNull(address))
        return null;
    try {
        return Process.pointerSize === 8 ? Number(address.readU64()) : address.readU32();
    }
    catch (_) {
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
    if (bytes.length === 0)
        return "";
    // Avoid presenting obviously binary data as text, but allow normal whitespace.
    for (let i = 0; i < bytes.length; i++) {
        const b = bytes[i];
        if (b === 0 || b < 0x09 || (b > 0x0d && b < 0x20))
            return null;
    }
    try {
        // TextDecoder is available in modern Frida runtimes. Keep a fallback for older runtimes.
        if (typeof TextDecoder !== "undefined") {
            return new TextDecoder("utf-8", { fatal: false }).decode(bytes);
        }
        let s = "";
        for (let i = 0; i < bytes.length; i++)
            s += String.fromCharCode(bytes[i]);
        return decodeURIComponent(escape(s));
    }
    catch (_) {
        return null;
    }
}
✄
{"version":3,"file":"format.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/utils/format.js"],"names":[],"mappings":"AAAA,MAAM,UAAU,QAAQ,CAAC,GAAG,EAAE,KAAK;IACjC,OAAO,MAAM,CAAC,SAAS,CAAC,cAAc,CAAC,IAAI,CAAC,GAAG,EAAE,KAAK,CAAC,CAAC,CAAC,CAAC,GAAG,CAAC,KAAK,CAAC,CAAC,CAAC,CAAC,UAAU,GAAG,KAAK,GAAG,GAAG,CAAC;AAClG,CAAC;AAED,MAAM,UAAU,WAAW,CAAC,GAAG,EAAE,KAAK;IACpC,IAAI,MAAM,CAAC,SAAS,CAAC,cAAc,CAAC,IAAI,CAAC,GAAG,EAAE,KAAK,CAAC;QAAE,OAAO,GAAG,CAAC,KAAK,CAAC,CAAC;IACxE,MAAM,KAAK,GAAG,EAAE,CAAC;IACjB,MAAM,CAAC,IAAI,CAAC,GAAG,CAAC,CAAC,OAAO,CAAC,CAAC,CAAC,EAAE;QAC3B,MAAM,GAAG,GAAG,MAAM,CAAC,CAAC,CAAC,CAAC;QACtB,IAAI,GAAG,KAAK,CAAC,IAAI,CAAC,KAAK,GAAG,GAAG,CAAC,KAAK,GAAG;YAAE,KAAK,CAAC,IAAI,CAAC,GAAG,CAAC,CAAC,CAAC,CAAC,CAAC;IAC7D,CAAC,CAAC,CAAC;IACH,OAAO,KAAK,CAAC,MAAM,GAAG,CAAC,CAAC,CAAC,CAAC,KAAK,CAAC,IAAI,CAAC,GAAG,CAAC,CAAC,CAAC,CAAC,UAAU,GAAG,KAAK,GAAG,GAAG,CAAC;AACvE,CAAC"}
✄
export function enumName(map, value) {
    return Object.prototype.hasOwnProperty.call(map, value) ? map[value] : "unknown(" + value + ")";
}
export function optionNames(map, value) {
    if (Object.prototype.hasOwnProperty.call(map, value))
        return map[value];
    const names = [];
    Object.keys(map).forEach(k => {
        const bit = Number(k);
        if (bit !== 0 && (value & bit) === bit)
            names.push(map[k]);
    });
    return names.length > 0 ? names.join("|") : "unknown(" + value + ")";
}
✄
{"version":3,"file":"native.js","sourceRoot":"C:/Users/rkornilov/Documents/tools/custom/Temsa/frida-scripts/","sources":["src/ios/ios.cypher/utils/native.js"],"names":[],"mappings":"AAAA,MAAM,UAAU,UAAU,CAAC,IAAI;IAC7B,IAAI;QACF,OAAO,MAAM,CAAC,sBAAsB,CAAC,IAAI,CAAC,CAAC;KAC5C;IAAC,OAAO,CAAC,EAAE;QACV,OAAO,IAAI,CAAC;KACb;AACH,CAAC;AAED,MAAM,UAAU,YAAY,CAAC,IAAI,EAAE,SAAS,EAAE,IAAI;IAChD,MAAM,OAAO,GAAG,UAAU,CAAC,IAAI,CAAC,CAAC;IACjC,IAAI,OAAO,KAAK,IAAI,EAAE;QACpB,IAAI,IAAI,IAAI,CAAC;YACX,IAAI,EAAE,iBAAiB;YACvB,QAAQ,EAAE,iBAAiB;YAC3B,KAAK,EAAE,gBAAgB;YACvB,GAAG,EAAE,IAAI;SACV,CAAC,CAAC;QACH,OAAO,KAAK,CAAC;KACd;IACD,WAAW,CAAC,MAAM,CAAC,OAAO,EAAE,SAAS,CAAC,CAAC;IACvC,IAAI,IAAI,IAAI,CAAC;QACX,IAAI,EAAE,iBAAiB;QACvB,QAAQ,EAAE,iBAAiB;QAC3B,KAAK,EAAE,eAAe;QACtB,GAAG,EAAE,IAAI;QACT,OAAO,EAAE,OAAO,CAAC,QAAQ,EAAE;KAC5B,CAAC,CAAC;IACH,OAAO,IAAI,CAAC;AACd,CAAC;AAED,MAAM,UAAU,KAAK,CAAC,KAAK;IACzB,IAAI;QACF,OAAO,KAAK,CAAC,OAAO,EAAE,CAAC;KACxB;IAAC,OAAO,CAAC,EAAE;QACV,OAAO,MAAM,CAAC,KAAK,CAAC,CAAC;KACtB;AACH,CAAC;AAED,MAAM,UAAU,MAAM,CAAC,KAAK;IAC1B,IAAI;QACF,OAAO,KAAK,CAAC,QAAQ,EAAE,CAAC;KACzB;IAAC,OAAO,CAAC,EAAE;QACV,OAAO,MAAM,CAAC,KAAK,CAAC,CAAC;KACtB;AACH,CAAC;AAED,MAAM,UAAU,MAAM,CAAC,KAAK;IAC1B,IAAI,KAAK,KAAK,SAAS,IAAI,KAAK,KAAK,IAAI;QAAE,OAAO,CAAC,CAAC;IACpD,IAAI;QACF,MAAM,CAAC,GAAG,KAAK,CAAC,QAAQ,EAAE,CAAC;QAC3B,OAAO,CAAC,GAAG,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC;KACtB;IAAC,OAAO,CAAC,EAAE;QACV,MAAM,CAAC,GAAG,MAAM,CAAC,KAAK,CAAC,CAAC;QACxB,OAAO,MAAM,CAAC,QAAQ,CAAC,CAAC,CAAC,IAAI,CAAC,GAAG,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC;KAC5C;AACH,CAAC;AAED,MAAM,UAAU,MAAM,CAAC,QAAQ;IAC7B,OAAO,QAAQ,KAAK,IAAI,IAAI,QAAQ,CAAC,MAAM,EAAE,CAAC;AAChD,CAAC"}
✄
export function findExport(name) {
    try {
        return Module.findGlobalExportByName(name);
    }
    catch (_) {
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
    }
    catch (_) {
        return Number(value);
    }
}
export function toUInt(value) {
    try {
        return value.toUInt32();
    }
    catch (_) {
        return Number(value);
    }
}
export function toSize(value) {
    if (value === undefined || value === null)
        return 0;
    try {
        const n = value.toUInt32();
        return n < 0 ? 0 : n;
    }
    catch (_) {
        const n = Number(value);
        return Number.isFinite(n) && n > 0 ? n : 0;
    }
}
export function isNull(ptrValue) {
    return ptrValue === null || ptrValue.isNull();
}