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
