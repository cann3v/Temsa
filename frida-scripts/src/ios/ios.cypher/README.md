# ios.cipher

## Структура

```text
src/
  agent.ios.basic.js          # единая точка входа
  config.js                   # настройки перехвата
  constants/commoncrypto.js   # CommonCrypto enum/const mappings
  modules/
    ios.cipher.js             # агрегатор криптографического модуля
    ios.crypto.js             # CCCrypt/CCCryptor*
    ios.hash.js               # CC_MD*/CC_SHA*
    ios.hmac.js               # CCHmac*
    ios.pbkdf.js              # CCKeyDerivationPBKDF/CCCalibratePBKDF
    ios.keychain.js           # SecItem*
  utils/
    native.js                 # Frida 17-compatible export lookup/helpers
    data.js                   # безопасное чтение памяти
    backtrace.js              # native backtrace
    cf.js                     # CoreFoundation C helpers без ObjC/Swift bridge
    format.js                 # enum/options formatting
```

## Подключение как модуль

```js
import { installIosCipherHooks } from "./modules/ios.cipher.js";

installModule("ios.cipher", installIosCipherHooks);
```

## Формат событий

Все события отправляются через `send(...)` в JSON payload с полями:

```json
{
  "schemaVersion": 1,
  "profileId": "ios.basic",
  "platform": "ios",
  "timestamp": "2026-05-20T00:00:00.000Z",
  "moduleId": "ios.cipher",
  "type": "telemetry.event",
  "category": "crypto",
  "event": "commoncrypto.cccrypt",
  "api": "CCCrypt"
}
```
