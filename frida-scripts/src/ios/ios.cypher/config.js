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
