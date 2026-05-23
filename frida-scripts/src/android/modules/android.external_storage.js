import Java from "frida-java-bridge";

const CATEGORY = "storage";
const EXTERNAL_PATH_PREFIXES = ["/sdcard", "/storage/emulated"];

function isExternalPath(path) {
    if (!path) {
        return false;
    }

    return EXTERNAL_PATH_PREFIXES.some(prefix => path.indexOf(prefix) === 0);
}

function getNativeBacktrace(context, maxLines = 15) {
    try {
        return Thread.backtrace(context, Backtracer.ACCURATE)
            .slice(0, maxLines)
            .map(DebugSymbol.fromAddress)
            .map(x => x.toString());
    } catch (e) {
        return [`native_backtrace_error: ${String(e)}`];
    }
}

function getJavaBacktrace(maxLines = 8) {
    if (!Java.available) {
        return [];
    }

    try {
        const Exception = Java.use("java.lang.Exception");
        const stackTrace = Exception.$new()
            .getStackTrace()
            .toString()
            .split(",");

        return stackTrace
            .slice(0, maxLines)
            .map(x => x.trim());
    } catch (e) {
        return [`java_backtrace_error: ${String(e)}`];
    }
}

function getContentValue(values, key) {
    try {
        const value = values.get(key);
        return value === null ? null : value.toString();
    } catch (e) {
        return null;
    }
}

function installLibcOpenHook(emit) {
    const libc = Process.findModuleByName("libc.so");

    if (libc === null) {
        emit({
            type: "script.warning",
            category: CATEGORY,
            message: "libc.so was not found"
        });
        return;
    }

    const open = libc.findExportByName("open");

    if (open === null) {
        emit({
            type: "script.warning",
            category: CATEGORY,
            message: "libc open export was not found"
        });
        return;
    }

    Interceptor.attach(open, {
        onEnter(args) {
            let path = null;

            try {
                path = args[0].readCString();
            } catch (e) {
                emit({
                    type: "script.error",
                    category: CATEGORY,
                    api: "libc.open",
                    message: `failed to read path: ${String(e)}`
                });
                return;
            }

            if (!isExternalPath(path)) {
                return;
            }

            emit({
                type: "finding.candidate",
                category: CATEGORY,
                event: "external_storage.open",
                api: "libc.open",
                operation: "open",
                path,
                backtraceType: "native",
                backtrace: getNativeBacktrace(this.context, 15)
            });
        }
    });

    emit({
        type: "script.ready",
        category: CATEGORY,
        hook: "libc.open"
    });
}

function installContentResolverHook(emit) {
    Java.perform(() => {
        const ContentResolver = Java.use("android.content.ContentResolver");

        const insert = ContentResolver.insert.overload(
            "android.net.Uri",
            "android.content.ContentValues");

        insert.implementation = function (uri, values) {
            const input = {
                uri: uri === null ? null : uri.toString(),
                displayName: getContentValue(values, "_display_name"),
                mimeType: getContentValue(values, "mime_type"),
                relativePath: getContentValue(values, "relative_path")
            };

            try {
                const result = insert.call(this, uri, values);

                emit({
                    type: "finding.candidate",
                    category: CATEGORY,
                    event: "external_storage.content_resolver_insert",
                    api: "ContentResolver.insert",
                    operation: "insert",
                    input,
                    resultUri: result === null ? null : result.toString(),
                    backtraceType: "java",
                    backtrace: getJavaBacktrace(8)
                });

                return result;
            } catch (e) {
                emit({
                    type: "script.error",
                    category: CATEGORY,
                    event: "external_storage.content_resolver_insert",
                    api: "ContentResolver.insert",
                    message: String(e),
                    input,
                    backtraceType: "java",
                    backtrace: getJavaBacktrace(8)
                });

                throw e;
            }
        };

        emit({
            type: "script.ready",
            category: CATEGORY,
            hook: "ContentResolver.insert"
        });
    });
}

export function installExternalStorageHooks({ emit }) {
    emit({
        type: "module.started",
        category: CATEGORY
    });

    installLibcOpenHook(emit);

    if (Java.available) {
        installContentResolverHook(emit);
    } else {
        emit({
            type: "script.warning",
            category: CATEGORY,
            message: "Java runtime is not available"
        });
    }
}
