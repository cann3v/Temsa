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
  } catch (e) {
    emit({
      type: "submodule.error",
      submoduleId,
      message: String(e),
      stack: e && e.stack ? String(e.stack) : null
    });
  }
}
