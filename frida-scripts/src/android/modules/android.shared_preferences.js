import Java from "frida-java-bridge";

const CATEGORY = "storage";

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

function stringifyValue(value) {
    if (value === null || value === undefined) {
        return null;
    }

    try {
        return value.toString();
    } catch (e) {
        return `<toString_error: ${String(e)}>`;
    }
}

function normalizeValue(value) {
    const stringValue = stringifyValue(value);

    return {
        value: stringValue,
        valueLength: stringValue === null ? null : stringValue.length
    };
}

function emitSharedPreferencesEvent(emit, event, operation, api, key, valueType, value, extra) {
    const normalized = normalizeValue(value);

    emit({
        type: "finding.candidate",
        category: CATEGORY,
        event,
        api,
        operation,
        key: key === null || key === undefined ? null : String(key),
        valueType,
        value: normalized.value,
        valueLength: normalized.valueLength,
        ...extra,
        backtraceType: "java",
        backtrace: getJavaBacktrace(8)
    });
}

function hookPutValue(emit, editorClass, methodName, valueType, overloadType) {
    const overload = editorClass[methodName].overload(
        "java.lang.String",
        overloadType);

    overload.implementation = function (key, value) {
        emitSharedPreferencesEvent(
            emit,
            "shared_preferences.write",
            "write",
            `SharedPreferences.Editor.${methodName}`,
            key,
            valueType,
            value,
            null);

        return overload.call(this, key, value);
    };
}

export function installSharedPreferencesHooks({ emit }) {
    emit({
        type: "module.started",
        category: CATEGORY
    });

    if (!Java.available) {
        emit({
            type: "script.warning",
            category: CATEGORY,
            message: "Java runtime is not available"
        });
        return;
    }

    Java.perform(() => {
        const ContextWrapper = Java.use("android.content.ContextWrapper");

        const getSharedPreferences = ContextWrapper.getSharedPreferences.overload(
            "java.lang.String",
            "int");

        getSharedPreferences.implementation = function (name, mode) {
            emit({
                type: "telemetry.event",
                category: CATEGORY,
                event: "shared_preferences.open",
                api: "ContextWrapper.getSharedPreferences",
                operation: "open",
                name: name === null ? null : String(name),
                mode,
                backtraceType: "java",
                backtrace: getJavaBacktrace(6)
            });

            return getSharedPreferences.call(this, name, mode);
        };

        const EditorImpl = Java.use("android.app.SharedPreferencesImpl$EditorImpl");

        hookPutValue(emit, EditorImpl, "putString", "string", "java.lang.String");
        hookPutValue(emit, EditorImpl, "putBoolean", "boolean", "boolean");
        hookPutValue(emit, EditorImpl, "putFloat", "float", "float");
        hookPutValue(emit, EditorImpl, "putInt", "int", "int");
        hookPutValue(emit, EditorImpl, "putLong", "long", "long");

        const putStringSet = EditorImpl.putStringSet.overload(
            "java.lang.String",
            "java.util.Set");

        putStringSet.implementation = function (key, value) {
            emitSharedPreferencesEvent(
                emit,
                "shared_preferences.write",
                "write",
                "SharedPreferences.Editor.putStringSet",
                key,
                "string_set",
                value,
                {
                    setSize: value === null ? null : value.size()
                });

            return putStringSet.call(this, key, value);
        };

        const SharedPreferencesImpl = Java.use("android.app.SharedPreferencesImpl");

        const getString = SharedPreferencesImpl.getString.overload(
            "java.lang.String",
            "java.lang.String");

        getString.implementation = function (key, defaultValue) {
            const result = getString.call(this, key, defaultValue);

            emitSharedPreferencesEvent(
                emit,
                "shared_preferences.read",
                "read",
                "SharedPreferences.getString",
                key,
                "string",
                result,
                {
                    defaultValueUsed: result === defaultValue
                });

            return result;
        };

        emit({
            type: "script.ready",
            category: CATEGORY,
            hooks: [
                "ContextWrapper.getSharedPreferences",
                "SharedPreferences.Editor.putString",
                "SharedPreferences.Editor.putBoolean",
                "SharedPreferences.Editor.putFloat",
                "SharedPreferences.Editor.putInt",
                "SharedPreferences.Editor.putLong",
                "SharedPreferences.Editor.putStringSet",
                "SharedPreferences.getString"
            ]
        });
    });
}
