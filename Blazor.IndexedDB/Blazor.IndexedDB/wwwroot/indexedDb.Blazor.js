﻿!function (e) {
    var t = {};
    function n(o) {
        if (t[o])
            return t[o].exports;
        var r = t[o] = {
            i: o,
            l: !1,
            exports: {}
        };
        return e[o].call(r.exports, r, r.exports, n),
            r.l = !0,
            r.exports
    }
    n.m = e,
        n.c = t,
        n.d = function (e, t, o) {
            n.o(e, t) || Object.defineProperty(e, t, {
                enumerable: !0,
                get: o
            })
        }
        ,
        n.r = function (e) {
            "undefined" != typeof Symbol && Symbol.toStringTag && Object.defineProperty(e, Symbol.toStringTag, {
                value: "Module"
            }),
                Object.defineProperty(e, "__esModule", {
                    value: !0
                })
        }
        ,
        n.t = function (e, t) {
            if (1 & t && (e = n(e)),
                8 & t)
                return e;
            if (4 & t && "object" == typeof e && e && e.__esModule)
                return e;
            var o = Object.create(null);
            if (n.r(o),
                Object.defineProperty(o, "default", {
                    enumerable: !0,
                    value: e
                }),
                2 & t && "string" != typeof e)
                for (var r in e)
                    n.d(o, r, function (t) {
                        return e[t]
                    }
                        .bind(null, r));
            return o
        }
        ,
        n.n = function (e) {
            var t = e && e.__esModule ? function () {
                return e.default
            }
                : function () {
                    return e
                }
                ;
            return n.d(t, "a", t),
                t
        }
        ,
        n.o = function (e, t) {
            return Object.prototype.hasOwnProperty.call(e, t)
        }
        ,
        n.p = "",
        n(n.s = 0)
}([function (e, t, n) {
    "use strict";
    Object.defineProperty(t, "__esModule", {
        value: !0
    });
    const o = n(1);
    var r;
    !function (e) {
        const t = {
            IndexedDbManager: new o.IndexedDbManager
        };
        e.initialise = function () {
            "undefined" == typeof window || window.TimeGhost ? window.TimeGhost = Object.assign(Object.assign({}, window.TimeGhost), t) : window.TimeGhost = Object.assign({}, t)
        }
    }(r || (r = {})),
        r.initialise()
}
    , function (e, t, n) {
        "use strict";
        var o = this && this.__awaiter || function (e, t, n, o) {
            return new (n || (n = Promise))((function (r, i) {
                function s(e) {
                    try {
                        a(o.next(e))
                    } catch (e) {
                        i(e)
                    }
                }
                function c(e) {
                    try {
                        a(o.throw(e))
                    } catch (e) {
                        i(e)
                    }
                }
                function a(e) {
                    var t;
                    e.done ? r(e.value) : (t = e.value,
                        t instanceof n ? t : new n((function (e) {
                            e(t)
                        }
                        ))).then(s, c)
                }
                a((o = o.apply(e, t || [])).next())
            }
            ))
        }
            ;
        Object.defineProperty(t, "__esModule", {
            value: !0
        });
        const r = n(2);
        t.IndexedDbManager = class {
            constructor() {
                this.dbInstance = void 0,
                    this.dotnetCallback = e => { }
                    ,
                    this.openDb = (e, t) => o(this, void 0, void 0, (function* () {
                        const n = e;
                        this.dotnetCallback = e => {
                            t.instance.invokeMethodAsync(t.methodName, e)
                        }
                            ;
                        try {
                            (!this.dbInstance || this.dbInstance.version < n.version) && (this.dbInstance && this.dbInstance.close(),
                                this.dbInstance = yield r.default.open(n.dbName, n.version, e => {
                                    this.upgradeDatabase(e, n)
                                }
                                ))
                        } catch (e) {
                            this.dbInstance = yield r.default.open(n.dbName)
                        }
                        return `IndexedDB ${e.dbName} opened`
                    }
                    )),
                    this.getDbInfo = e => o(this, void 0, void 0, (function* () {
                        this.dbInstance || (this.dbInstance = yield r.default.open(e));
                        const t = this.dbInstance;
                        return {
                            version: t.version,
                            storeNames: (e => {
                                let t = [];
                                for (var n = 0; n < e.length; n++)
                                    t.push(e[n]);
                                return t
                            }
                            )(t.objectStoreNames)
                        }
                    }
                    )),
                    this.deleteDb = e => o(this, void 0, void 0, (function* () {
                        return this.dbInstance.close(),
                            yield r.default.delete(e),
                            this.dbInstance = void 0,
                            `The database ${e} has been deleted`
                    }
                    )),
                    this.addRecord = e => o(this, void 0, void 0, (function* () {
                        const t = e.storename;
                        let n = e.data;
                        const o = this.getTransaction(this.dbInstance, t, "readwrite").objectStore(t);
                        return n = this.checkForKeyPath(o, n),
                            `Added new record with id ${yield o.add(n, e.key)}`
                    }
                    )),
                    this.updateRecord = e => o(this, void 0, void 0, (function* () {
                        const t = e.storename
                            , n = this.getTransaction(this.dbInstance, t, "readwrite");
                        return `updated record with id ${yield n.objectStore(t).put(e.data, e.key)}`
                    }
                    )),
                    this.getRecords = e => o(this, void 0, void 0, (function* () {
                        const t = this.getTransaction(this.dbInstance, e, "readonly");
                        let n = yield t.objectStore(e).getAll();
                        return yield t.complete,
                            n
                    }
                    )),
                    this.clearStore = e => o(this, void 0, void 0, (function* () {
                        const t = this.getTransaction(this.dbInstance, e, "readwrite");
                        return yield t.objectStore(e).clear(),
                            yield t.complete,
                            `Store ${e} cleared`
                    }
                    )),
                    this.getRecordByIndex = e => o(this, void 0, void 0, (function* () {
                        const t = this.getTransaction(this.dbInstance, e.storename, "readonly")
                            , n = yield t.objectStore(e.storename).index(e.indexName).get(e.queryValue);
                        return yield t.complete,
                            n
                    }
                    )),
                    this.getAllRecordsByIndex = e => o(this, void 0, void 0, (function* () {
                        const t = this.getTransaction(this.dbInstance, e.storename, "readonly");
                        let n = [];
                        return t.objectStore(e.storename).index(e.indexName).iterateCursor(t => {
                            t && (t.key === e.queryValue && n.push(t.value),
                                t.continue())
                        }
                        ),
                            yield t.complete,
                            n
                    }
                    )),
                    this.getRecordById = (e, t) => o(this, void 0, void 0, (function* () {
                        const n = this.getTransaction(this.dbInstance, e, "readonly");
                        return yield n.objectStore(e).get(t)
                    }
                    )),
                    this.deleteRecord = (e, t) => o(this, void 0, void 0, (function* () {
                        const n = this.getTransaction(this.dbInstance, e, "readwrite");
                        return yield n.objectStore(e).delete(t),
                            `Record with id: ${t} deleted`
                    }
                    ))
            }
            getTransaction(e, t, n) {
                const o = e.transaction(t, n);
                return o.complete.catch(e => {
                    console.log(e.message)
                }
                ),
                    o
            }
            checkForKeyPath(e, t) {
                if (!e.autoIncrement || !e.keyPath)
                    return t;
                if ("string" != typeof e.keyPath)
                    return t;
                const n = e.keyPath;
                return t[n] || delete t[n],
                    t
            }
            upgradeDatabase(e, t) {
                if (e.oldVersion < t.version && t.stores)
                    for (var n of t.stores)
                        e.objectStoreNames.contains(n.name) || (this.addNewStore(e, n),
                            this.dotnetCallback(`store added ${n.name}: db version: ${t.version}`))
            }
            addNewStore(e, t) {
                let n = t.primaryKey;
                n || (n = {
                    name: "id",
                    keyPath: "id",
                    auto: !0
                });
                const o = e.createObjectStore(t.name, {
                    keyPath: n.keyPath,
                    autoIncrement: n.auto
                });
                for (var r of t.indexes)
                    o.createIndex(r.name, r.keyPath, {
                        unique: r.unique
                    })
            }
        }
    }
    , function (e, t, n) {
        "use strict";
        !function () {
            function t(e) {
                return Array.prototype.slice.call(e)
            }
            function n(e) {
                return new Promise((function (t, n) {
                    e.onsuccess = function () {
                        t(e.result)
                    }
                        ,
                        e.onerror = function () {
                            n(e.error)
                        }
                }
                ))
            }
            function o(e, t, o) {
                var r, i = new Promise((function (i, s) {
                    n(r = e[t].apply(e, o)).then(i, s)
                }
                ));
                return i.request = r,
                    i
            }
            function r(e, t, n) {
                var r = o(e, t, n);
                return r.then((function (e) {
                    if (e)
                        return new d(e, r.request)
                }
                ))
            }
            function i(e, t, n) {
                n.forEach((function (n) {
                    Object.defineProperty(e.prototype, n, {
                        get: function () {
                            return this[t][n]
                        },
                        set: function (e) {
                            this[t][n] = e
                        }
                    })
                }
                ))
            }
            function s(e, t, n, r) {
                r.forEach((function (r) {
                    r in n.prototype && (e.prototype[r] = function () {
                        return o(this[t], r, arguments)
                    }
                    )
                }
                ))
            }
            function c(e, t, n, o) {
                o.forEach((function (o) {
                    o in n.prototype && (e.prototype[o] = function () {
                        return this[t][o].apply(this[t], arguments)
                    }
                    )
                }
                ))
            }
            function a(e, t, n, o) {
                o.forEach((function (o) {
                    o in n.prototype && (e.prototype[o] = function () {
                        return r(this[t], o, arguments)
                    }
                    )
                }
                ))
            }
            function u(e) {
                this._index = e
            }
            function d(e, t) {
                this._cursor = e,
                    this._request = t
            }
            function l(e) {
                this._store = e
            }
            function h(e) {
                this._tx = e,
                    this.complete = new Promise((function (t, n) {
                        e.oncomplete = function () {
                            t()
                        }
                            ,
                            e.onerror = function () {
                                n(e.error)
                            }
                            ,
                            e.onabort = function () {
                                n(e.error)
                            }
                    }
                    ))
            }
            function f(e, t, n) {
                this._db = e,
                    this.oldVersion = t,
                    this.transaction = new h(n)
            }
            function p(e) {
                this._db = e
            }
            i(u, "_index", ["name", "keyPath", "multiEntry", "unique"]),
                s(u, "_index", IDBIndex, ["get", "getKey", "getAll", "getAllKeys", "count"]),
                a(u, "_index", IDBIndex, ["openCursor", "openKeyCursor"]),
                i(d, "_cursor", ["direction", "key", "primaryKey", "value"]),
                s(d, "_cursor", IDBCursor, ["update", "delete"]),
                ["advance", "continue", "continuePrimaryKey"].forEach((function (e) {
                    e in IDBCursor.prototype && (d.prototype[e] = function () {
                        var t = this
                            , o = arguments;
                        return Promise.resolve().then((function () {
                            return t._cursor[e].apply(t._cursor, o),
                                n(t._request).then((function (e) {
                                    if (e)
                                        return new d(e, t._request)
                                }
                                ))
                        }
                        ))
                    }
                    )
                }
                )),
                l.prototype.createIndex = function () {
                    return new u(this._store.createIndex.apply(this._store, arguments))
                }
                ,
                l.prototype.index = function () {
                    return new u(this._store.index.apply(this._store, arguments))
                }
                ,
                i(l, "_store", ["name", "keyPath", "indexNames", "autoIncrement"]),
                s(l, "_store", IDBObjectStore, ["put", "add", "delete", "clear", "get", "getAll", "getKey", "getAllKeys", "count"]),
                a(l, "_store", IDBObjectStore, ["openCursor", "openKeyCursor"]),
                c(l, "_store", IDBObjectStore, ["deleteIndex"]),
                h.prototype.objectStore = function () {
                    return new l(this._tx.objectStore.apply(this._tx, arguments))
                }
                ,
                i(h, "_tx", ["objectStoreNames", "mode"]),
                c(h, "_tx", IDBTransaction, ["abort"]),
                f.prototype.createObjectStore = function () {
                    return new l(this._db.createObjectStore.apply(this._db, arguments))
                }
                ,
                i(f, "_db", ["name", "version", "objectStoreNames"]),
                c(f, "_db", IDBDatabase, ["deleteObjectStore", "close"]),
                p.prototype.transaction = function () {
                    return new h(this._db.transaction.apply(this._db, arguments))
                }
                ,
                i(p, "_db", ["name", "version", "objectStoreNames"]),
                c(p, "_db", IDBDatabase, ["close"]),
                ["openCursor", "openKeyCursor"].forEach((function (e) {
                    [l, u].forEach((function (n) {
                        e in n.prototype && (n.prototype[e.replace("open", "iterate")] = function () {
                            var n = t(arguments)
                                , o = n[n.length - 1]
                                , r = this._store || this._index
                                , i = r[e].apply(r, n.slice(0, -1));
                            i.onsuccess = function () {
                                o(i.result)
                            }
                        }
                        )
                    }
                    ))
                }
                )),
                [u, l].forEach((function (e) {
                    e.prototype.getAll || (e.prototype.getAll = function (e, t) {
                        var n = this
                            , o = [];
                        return new Promise((function (r) {
                            n.iterateCursor(e, (function (e) {
                                e ? (o.push(e.value),
                                    void 0 === t || o.length != t ? e.continue() : r(o)) : r(o)
                            }
                            ))
                        }
                        ))
                    }
                    )
                }
                ));
            var y = {
                open: function (e, t, n) {
                    var r = o(indexedDB, "open", [e, t])
                        , i = r.request;
                    return i && (i.onupgradeneeded = function (e) {
                        n && n(new f(i.result, e.oldVersion, i.transaction))
                    }
                    ),
                        r.then((function (e) {
                            return new p(e)
                        }
                        ))
                },
                delete: function (e) {
                    return o(indexedDB, "deleteDatabase", [e])
                }
            };
            e.exports = y,
                e.exports.default = e.exports
        }()
    }
]);
