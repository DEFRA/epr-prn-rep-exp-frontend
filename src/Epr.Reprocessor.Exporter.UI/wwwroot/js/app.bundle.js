(() => {
  // node_modules/govuk-frontend/govuk-esm/common/index.mjs
  function nodeListForEach(nodes, callback) {
    if (window.NodeList.prototype.forEach) {
      return nodes.forEach(callback);
    }
    for (var i = 0; i < nodes.length; i++) {
      callback.call(window, nodes[i], i, nodes);
    }
  }
  function generateUniqueID() {
    var d = (/* @__PURE__ */ new Date()).getTime();
    if (typeof window.performance !== "undefined" && typeof window.performance.now === "function") {
      d += window.performance.now();
    }
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function(c) {
      var r = (d + Math.random() * 16) % 16 | 0;
      d = Math.floor(d / 16);
      return (c === "x" ? r : r & 3 | 8).toString(16);
    });
  }
  function mergeConfigs() {
    var flattenObject = function(configObject) {
      var flattenedObject = {};
      var flattenLoop = function(obj2, prefix) {
        for (var key2 in obj2) {
          if (!Object.prototype.hasOwnProperty.call(obj2, key2)) {
            continue;
          }
          var value = obj2[key2];
          var prefixedKey = prefix ? prefix + "." + key2 : key2;
          if (typeof value === "object") {
            flattenLoop(value, prefixedKey);
          } else {
            flattenedObject[prefixedKey] = value;
          }
        }
      };
      flattenLoop(configObject);
      return flattenedObject;
    };
    var formattedConfigObject = {};
    for (var i = 0; i < arguments.length; i++) {
      var obj = flattenObject(arguments[i]);
      for (var key in obj) {
        if (Object.prototype.hasOwnProperty.call(obj, key)) {
          formattedConfigObject[key] = obj[key];
        }
      }
    }
    return formattedConfigObject;
  }
  function extractConfigByNamespace(configObject, namespace) {
    if (!configObject || typeof configObject !== "object") {
      throw new Error('Provide a `configObject` of type "object".');
    }
    if (!namespace || typeof namespace !== "string") {
      throw new Error('Provide a `namespace` of type "string" to filter the `configObject` by.');
    }
    var newObject = {};
    for (var key in configObject) {
      var keyParts = key.split(".");
      if (Object.prototype.hasOwnProperty.call(configObject, key) && keyParts[0] === namespace) {
        if (keyParts.length > 1) {
          keyParts.shift();
        }
        var newKey = keyParts.join(".");
        newObject[newKey] = configObject[key];
      }
    }
    return newObject;
  }

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Object/defineProperty.mjs
  (function(undefined2) {
    var detect = (
      // In IE8, defineProperty could only act on DOM elements, so full support
      // for the feature requires the ability to set a property on an arbitrary object
      "defineProperty" in Object && function() {
        try {
          var a = {};
          Object.defineProperty(a, "test", { value: 42 });
          return true;
        } catch (e) {
          return false;
        }
      }()
    );
    if (detect) return;
    (function(nativeDefineProperty) {
      var supportsAccessors = Object.prototype.hasOwnProperty("__defineGetter__");
      var ERR_ACCESSORS_NOT_SUPPORTED = "Getters & setters cannot be defined on this javascript engine";
      var ERR_VALUE_ACCESSORS = "A property cannot both have accessors and be writable or have a value";
      Object.defineProperty = function defineProperty(object, property, descriptor) {
        if (nativeDefineProperty && (object === window || object === document || object === Element.prototype || object instanceof Element)) {
          return nativeDefineProperty(object, property, descriptor);
        }
        if (object === null || !(object instanceof Object || typeof object === "object")) {
          throw new TypeError("Object.defineProperty called on non-object");
        }
        if (!(descriptor instanceof Object)) {
          throw new TypeError("Property description must be an object");
        }
        var propertyString = String(property);
        var hasValueOrWritable = "value" in descriptor || "writable" in descriptor;
        var getterType = "get" in descriptor && typeof descriptor.get;
        var setterType = "set" in descriptor && typeof descriptor.set;
        if (getterType) {
          if (getterType !== "function") {
            throw new TypeError("Getter must be a function");
          }
          if (!supportsAccessors) {
            throw new TypeError(ERR_ACCESSORS_NOT_SUPPORTED);
          }
          if (hasValueOrWritable) {
            throw new TypeError(ERR_VALUE_ACCESSORS);
          }
          Object.__defineGetter__.call(object, propertyString, descriptor.get);
        } else {
          object[propertyString] = descriptor.value;
        }
        if (setterType) {
          if (setterType !== "function") {
            throw new TypeError("Setter must be a function");
          }
          if (!supportsAccessors) {
            throw new TypeError(ERR_ACCESSORS_NOT_SUPPORTED);
          }
          if (hasValueOrWritable) {
            throw new TypeError(ERR_VALUE_ACCESSORS);
          }
          Object.__defineSetter__.call(object, propertyString, descriptor.set);
        }
        if ("value" in descriptor) {
          object[propertyString] = descriptor.value;
        }
        return object;
      };
    })(Object.defineProperty);
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Document.mjs
  (function(undefined2) {
    var detect = "Document" in this;
    if (detect) return;
    if (typeof WorkerGlobalScope === "undefined" && typeof importScripts !== "function") {
      if (this.HTMLDocument) {
        this.Document = this.HTMLDocument;
      } else {
        this.Document = this.HTMLDocument = document.constructor = new Function("return function Document() {}")();
        this.Document.prototype = document;
      }
    }
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Element.mjs
  (function(undefined2) {
    var detect = "Element" in this && "HTMLElement" in this;
    if (detect) return;
    (function() {
      if (window.Element && !window.HTMLElement) {
        window.HTMLElement = window.Element;
        return;
      }
      window.Element = window.HTMLElement = new Function("return function Element() {}")();
      var vbody = document.appendChild(document.createElement("body"));
      var frame = vbody.appendChild(document.createElement("iframe"));
      var frameDocument = frame.contentWindow.document;
      var prototype = Element.prototype = frameDocument.appendChild(frameDocument.createElement("*"));
      var cache = {};
      var shiv = function(element, deep) {
        var childNodes = element.childNodes || [], index = -1, key, value, childNode;
        if (element.nodeType === 1 && element.constructor !== Element) {
          element.constructor = Element;
          for (key in cache) {
            value = cache[key];
            element[key] = value;
          }
        }
        while (childNode = deep && childNodes[++index]) {
          shiv(childNode, deep);
        }
        return element;
      };
      var elements = document.getElementsByTagName("*");
      var nativeCreateElement = document.createElement;
      var interval;
      var loopLimit = 100;
      prototype.attachEvent("onpropertychange", function(event) {
        var propertyName = event.propertyName, nonValue = !cache.hasOwnProperty(propertyName), newValue = prototype[propertyName], oldValue = cache[propertyName], index = -1, element;
        while (element = elements[++index]) {
          if (element.nodeType === 1) {
            if (nonValue || element[propertyName] === oldValue) {
              element[propertyName] = newValue;
            }
          }
        }
        cache[propertyName] = newValue;
      });
      prototype.constructor = Element;
      if (!prototype.hasAttribute) {
        prototype.hasAttribute = function hasAttribute(name) {
          return this.getAttribute(name) !== null;
        };
      }
      function bodyCheck() {
        if (!loopLimit--) clearTimeout(interval);
        if (document.body && !document.body.prototype && /(complete|interactive)/.test(document.readyState)) {
          shiv(document, true);
          if (interval && document.body.prototype) clearTimeout(interval);
          return !!document.body.prototype;
        }
        return false;
      }
      if (!bodyCheck()) {
        document.onreadystatechange = bodyCheck;
        interval = setInterval(bodyCheck, 25);
      }
      document.createElement = function createElement(nodeName) {
        var element = nativeCreateElement(String(nodeName).toLowerCase());
        return shiv(element);
      };
      document.removeChild(vbody);
    })();
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Element/prototype/dataset.mjs
  (function(undefined2) {
    var detect = function() {
      if (!document.documentElement.dataset) {
        return false;
      }
      var el = document.createElement("div");
      el.setAttribute("data-a-b", "c");
      return el.dataset && el.dataset.aB == "c";
    }();
    if (detect) return;
    Object.defineProperty(Element.prototype, "dataset", {
      get: function() {
        var element = this;
        var attributes = this.attributes;
        var map = {};
        for (var i = 0; i < attributes.length; i++) {
          var attribute = attributes[i];
          if (attribute && attribute.name && /^data-\w[.\w-]*$/.test(attribute.name)) {
            var name = attribute.name;
            var value = attribute.value;
            var propName = name.substr(5).replace(/-./g, function(prop) {
              return prop.charAt(1).toUpperCase();
            });
            if ("__defineGetter__" in Object.prototype && "__defineSetter__" in Object.prototype) {
              Object.defineProperty(map, propName, {
                enumerable: true,
                get: function() {
                  return this.value;
                }.bind({ value: value || "" }),
                set: function setter(name2, value2) {
                  if (typeof value2 !== "undefined") {
                    this.setAttribute(name2, value2);
                  } else {
                    this.removeAttribute(name2);
                  }
                }.bind(element, name)
              });
            } else {
              map[propName] = value;
            }
          }
        }
        return map;
      }
    });
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/String/prototype/trim.mjs
  (function(undefined2) {
    var detect = "trim" in String.prototype;
    if (detect) return;
    String.prototype.trim = function() {
      return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "");
    };
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/common/normalise-dataset.mjs
  function normaliseString(value) {
    if (typeof value !== "string") {
      return value;
    }
    var trimmedValue = value.trim();
    if (trimmedValue === "true") {
      return true;
    }
    if (trimmedValue === "false") {
      return false;
    }
    if (trimmedValue.length > 0 && isFinite(Number(trimmedValue))) {
      return Number(trimmedValue);
    }
    return value;
  }
  function normaliseDataset(dataset) {
    var out = {};
    for (var key in dataset) {
      out[key] = normaliseString(dataset[key]);
    }
    return out;
  }

  // node_modules/govuk-frontend/govuk-esm/i18n.mjs
  function I18n(translations, config) {
    this.translations = translations || {};
    this.locale = config && config.locale || document.documentElement.lang || "en";
  }
  I18n.prototype.t = function(lookupKey, options) {
    if (!lookupKey) {
      throw new Error("i18n: lookup key missing");
    }
    if (options && typeof options.count === "number") {
      lookupKey = lookupKey + "." + this.getPluralSuffix(lookupKey, options.count);
    }
    var translationString = this.translations[lookupKey];
    if (typeof translationString === "string") {
      if (translationString.match(/%{(.\S+)}/)) {
        if (!options) {
          throw new Error("i18n: cannot replace placeholders in string if no option data provided");
        }
        return this.replacePlaceholders(translationString, options);
      } else {
        return translationString;
      }
    } else {
      return lookupKey;
    }
  };
  I18n.prototype.replacePlaceholders = function(translationString, options) {
    var formatter;
    if (this.hasIntlNumberFormatSupport()) {
      formatter = new Intl.NumberFormat(this.locale);
    }
    return translationString.replace(
      /%{(.\S+)}/g,
      /**
       * Replace translation string placeholders
       *
       * @param {string} placeholderWithBraces - Placeholder with braces
       * @param {string} placeholderKey - Placeholder key
       * @returns {string} Placeholder value
       */
      function(placeholderWithBraces, placeholderKey) {
        if (Object.prototype.hasOwnProperty.call(options, placeholderKey)) {
          var placeholderValue = options[placeholderKey];
          if (placeholderValue === false || typeof placeholderValue !== "number" && typeof placeholderValue !== "string") {
            return "";
          }
          if (typeof placeholderValue === "number") {
            return formatter ? formatter.format(placeholderValue) : placeholderValue.toString();
          }
          return placeholderValue;
        } else {
          throw new Error("i18n: no data found to replace " + placeholderWithBraces + " placeholder in string");
        }
      }
    );
  };
  I18n.prototype.hasIntlPluralRulesSupport = function() {
    return Boolean(window.Intl && ("PluralRules" in window.Intl && Intl.PluralRules.supportedLocalesOf(this.locale).length));
  };
  I18n.prototype.hasIntlNumberFormatSupport = function() {
    return Boolean(window.Intl && ("NumberFormat" in window.Intl && Intl.NumberFormat.supportedLocalesOf(this.locale).length));
  };
  I18n.prototype.getPluralSuffix = function(lookupKey, count) {
    count = Number(count);
    if (!isFinite(count)) {
      return "other";
    }
    var preferredForm;
    if (this.hasIntlPluralRulesSupport()) {
      preferredForm = new Intl.PluralRules(this.locale).select(count);
    } else {
      preferredForm = this.selectPluralFormUsingFallbackRules(count);
    }
    if (lookupKey + "." + preferredForm in this.translations) {
      return preferredForm;
    } else if (lookupKey + ".other" in this.translations) {
      if (console && "warn" in console) {
        console.warn('i18n: Missing plural form ".' + preferredForm + '" for "' + this.locale + '" locale. Falling back to ".other".');
      }
      return "other";
    } else {
      throw new Error(
        'i18n: Plural form ".other" is required for "' + this.locale + '" locale'
      );
    }
  };
  I18n.prototype.selectPluralFormUsingFallbackRules = function(count) {
    count = Math.abs(Math.floor(count));
    var ruleset = this.getPluralRulesForLocale();
    if (ruleset) {
      return I18n.pluralRules[ruleset](count);
    }
    return "other";
  };
  I18n.prototype.getPluralRulesForLocale = function() {
    var locale = this.locale;
    var localeShort = locale.split("-")[0];
    for (var pluralRule in I18n.pluralRulesMap) {
      if (Object.prototype.hasOwnProperty.call(I18n.pluralRulesMap, pluralRule)) {
        var languages = I18n.pluralRulesMap[pluralRule];
        for (var i = 0; i < languages.length; i++) {
          if (languages[i] === locale || languages[i] === localeShort) {
            return pluralRule;
          }
        }
      }
    }
  };
  I18n.pluralRulesMap = {
    arabic: ["ar"],
    chinese: ["my", "zh", "id", "ja", "jv", "ko", "ms", "th", "vi"],
    french: ["hy", "bn", "fr", "gu", "hi", "fa", "pa", "zu"],
    german: [
      "af",
      "sq",
      "az",
      "eu",
      "bg",
      "ca",
      "da",
      "nl",
      "en",
      "et",
      "fi",
      "ka",
      "de",
      "el",
      "hu",
      "lb",
      "no",
      "so",
      "sw",
      "sv",
      "ta",
      "te",
      "tr",
      "ur"
    ],
    irish: ["ga"],
    russian: ["ru", "uk"],
    scottish: ["gd"],
    spanish: ["pt-PT", "it", "es"],
    welsh: ["cy"]
  };
  I18n.pluralRules = {
    /* eslint-disable jsdoc/require-jsdoc */
    arabic: function(n) {
      if (n === 0) {
        return "zero";
      }
      if (n === 1) {
        return "one";
      }
      if (n === 2) {
        return "two";
      }
      if (n % 100 >= 3 && n % 100 <= 10) {
        return "few";
      }
      if (n % 100 >= 11 && n % 100 <= 99) {
        return "many";
      }
      return "other";
    },
    chinese: function() {
      return "other";
    },
    french: function(n) {
      return n === 0 || n === 1 ? "one" : "other";
    },
    german: function(n) {
      return n === 1 ? "one" : "other";
    },
    irish: function(n) {
      if (n === 1) {
        return "one";
      }
      if (n === 2) {
        return "two";
      }
      if (n >= 3 && n <= 6) {
        return "few";
      }
      if (n >= 7 && n <= 10) {
        return "many";
      }
      return "other";
    },
    russian: function(n) {
      var lastTwo = n % 100;
      var last = lastTwo % 10;
      if (last === 1 && lastTwo !== 11) {
        return "one";
      }
      if (last >= 2 && last <= 4 && !(lastTwo >= 12 && lastTwo <= 14)) {
        return "few";
      }
      if (last === 0 || last >= 5 && last <= 9 || lastTwo >= 11 && lastTwo <= 14) {
        return "many";
      }
      return "other";
    },
    scottish: function(n) {
      if (n === 1 || n === 11) {
        return "one";
      }
      if (n === 2 || n === 12) {
        return "two";
      }
      if (n >= 3 && n <= 10 || n >= 13 && n <= 19) {
        return "few";
      }
      return "other";
    },
    spanish: function(n) {
      if (n === 1) {
        return "one";
      }
      if (n % 1e6 === 0 && n !== 0) {
        return "many";
      }
      return "other";
    },
    welsh: function(n) {
      if (n === 0) {
        return "zero";
      }
      if (n === 1) {
        return "one";
      }
      if (n === 2) {
        return "two";
      }
      if (n === 3) {
        return "few";
      }
      if (n === 6) {
        return "many";
      }
      return "other";
    }
    /* eslint-enable jsdoc/require-jsdoc */
  };

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/DOMTokenList.mjs
  (function(undefined2) {
    var detect = "DOMTokenList" in this && function(x) {
      return "classList" in x ? !x.classList.toggle("x", false) && !x.className : true;
    }(document.createElement("x"));
    if (detect) return;
    (function(global2) {
      var nativeImpl = "DOMTokenList" in global2 && global2.DOMTokenList;
      if (!nativeImpl || !!document.createElementNS && !!document.createElementNS("http://www.w3.org/2000/svg", "svg") && !(document.createElementNS("http://www.w3.org/2000/svg", "svg").classList instanceof DOMTokenList)) {
        global2.DOMTokenList = function() {
          var dpSupport = true;
          var defineGetter = function(object, name, fn, configurable) {
            if (Object.defineProperty)
              Object.defineProperty(object, name, {
                configurable: false === dpSupport ? true : !!configurable,
                get: fn
              });
            else object.__defineGetter__(name, fn);
          };
          try {
            defineGetter({}, "support");
          } catch (e) {
            dpSupport = false;
          }
          var _DOMTokenList = function(el, prop) {
            var that = this;
            var tokens = [];
            var tokenMap = {};
            var length = 0;
            var maxLength = 0;
            var addIndexGetter = function(i) {
              defineGetter(that, i, function() {
                preop();
                return tokens[i];
              }, false);
            };
            var reindex = function() {
              if (length >= maxLength)
                for (; maxLength < length; ++maxLength) {
                  addIndexGetter(maxLength);
                }
            };
            var preop = function() {
              var error;
              var i;
              var args = arguments;
              var rSpace = /\s+/;
              if (args.length) {
                for (i = 0; i < args.length; ++i)
                  if (rSpace.test(args[i])) {
                    error = new SyntaxError('String "' + args[i] + '" contains an invalid character');
                    error.code = 5;
                    error.name = "InvalidCharacterError";
                    throw error;
                  }
              }
              if (typeof el[prop] === "object") {
                tokens = ("" + el[prop].baseVal).replace(/^\s+|\s+$/g, "").split(rSpace);
              } else {
                tokens = ("" + el[prop]).replace(/^\s+|\s+$/g, "").split(rSpace);
              }
              if ("" === tokens[0]) tokens = [];
              tokenMap = {};
              for (i = 0; i < tokens.length; ++i)
                tokenMap[tokens[i]] = true;
              length = tokens.length;
              reindex();
            };
            preop();
            defineGetter(that, "length", function() {
              preop();
              return length;
            });
            that.toLocaleString = that.toString = function() {
              preop();
              return tokens.join(" ");
            };
            that.item = function(idx) {
              preop();
              return tokens[idx];
            };
            that.contains = function(token) {
              preop();
              return !!tokenMap[token];
            };
            that.add = function() {
              preop.apply(that, args = arguments);
              for (var args, token, i = 0, l = args.length; i < l; ++i) {
                token = args[i];
                if (!tokenMap[token]) {
                  tokens.push(token);
                  tokenMap[token] = true;
                }
              }
              if (length !== tokens.length) {
                length = tokens.length >>> 0;
                if (typeof el[prop] === "object") {
                  el[prop].baseVal = tokens.join(" ");
                } else {
                  el[prop] = tokens.join(" ");
                }
                reindex();
              }
            };
            that.remove = function() {
              preop.apply(that, args = arguments);
              for (var args, ignore = {}, i = 0, t = []; i < args.length; ++i) {
                ignore[args[i]] = true;
                delete tokenMap[args[i]];
              }
              for (i = 0; i < tokens.length; ++i)
                if (!ignore[tokens[i]]) t.push(tokens[i]);
              tokens = t;
              length = t.length >>> 0;
              if (typeof el[prop] === "object") {
                el[prop].baseVal = tokens.join(" ");
              } else {
                el[prop] = tokens.join(" ");
              }
              reindex();
            };
            that.toggle = function(token, force) {
              preop.apply(that, [token]);
              if (undefined2 !== force) {
                if (force) {
                  that.add(token);
                  return true;
                } else {
                  that.remove(token);
                  return false;
                }
              }
              if (tokenMap[token]) {
                that.remove(token);
                return false;
              }
              that.add(token);
              return true;
            };
            return that;
          };
          return _DOMTokenList;
        }();
      }
      (function() {
        var e = document.createElement("span");
        if (!("classList" in e)) return;
        e.classList.toggle("x", false);
        if (!e.classList.contains("x")) return;
        e.classList.constructor.prototype.toggle = function toggle(token) {
          var force = arguments[1];
          if (force === undefined2) {
            var add = !this.contains(token);
            this[add ? "add" : "remove"](token);
            return add;
          }
          force = !!force;
          this[force ? "add" : "remove"](token);
          return force;
        };
      })();
      (function() {
        var e = document.createElement("span");
        if (!("classList" in e)) return;
        e.classList.add("a", "b");
        if (e.classList.contains("b")) return;
        var native = e.classList.constructor.prototype.add;
        e.classList.constructor.prototype.add = function() {
          var args = arguments;
          var l = arguments.length;
          for (var i = 0; i < l; i++) {
            native.call(this, args[i]);
          }
        };
      })();
      (function() {
        var e = document.createElement("span");
        if (!("classList" in e)) return;
        e.classList.add("a");
        e.classList.add("b");
        e.classList.remove("a", "b");
        if (!e.classList.contains("b")) return;
        var native = e.classList.constructor.prototype.remove;
        e.classList.constructor.prototype.remove = function() {
          var args = arguments;
          var l = arguments.length;
          for (var i = 0; i < l; i++) {
            native.call(this, args[i]);
          }
        };
      })();
    })(this);
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Element/prototype/classList.mjs
  (function(undefined2) {
    var detect = "document" in this && "classList" in document.documentElement && "Element" in this && "classList" in Element.prototype && function() {
      var e = document.createElement("span");
      e.classList.add("a", "b");
      return e.classList.contains("b");
    }();
    if (detect) return;
    (function(global2) {
      var dpSupport = true;
      var defineGetter = function(object, name, fn, configurable) {
        if (Object.defineProperty)
          Object.defineProperty(object, name, {
            configurable: false === dpSupport ? true : !!configurable,
            get: fn
          });
        else object.__defineGetter__(name, fn);
      };
      try {
        defineGetter({}, "support");
      } catch (e) {
        dpSupport = false;
      }
      var addProp = function(o, name, attr) {
        defineGetter(o.prototype, name, function() {
          var tokenList;
          var THIS = this, gibberishProperty = "__defineGetter__DEFINE_PROPERTY" + name;
          if (THIS[gibberishProperty]) return tokenList;
          THIS[gibberishProperty] = true;
          if (false === dpSupport) {
            var visage;
            var mirror = addProp.mirror || document.createElement("div");
            var reflections = mirror.childNodes;
            var l = reflections.length;
            for (var i = 0; i < l; ++i)
              if (reflections[i]._R === THIS) {
                visage = reflections[i];
                break;
              }
            visage || (visage = mirror.appendChild(document.createElement("div")));
            tokenList = DOMTokenList.call(visage, THIS, attr);
          } else tokenList = new DOMTokenList(THIS, attr);
          defineGetter(THIS, name, function() {
            return tokenList;
          });
          delete THIS[gibberishProperty];
          return tokenList;
        }, true);
      };
      addProp(global2.Element, "classList", "className");
      addProp(global2.HTMLElement, "classList", "className");
      addProp(global2.HTMLLinkElement, "relList", "rel");
      addProp(global2.HTMLAnchorElement, "relList", "rel");
      addProp(global2.HTMLAreaElement, "relList", "rel");
    })(this);
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Element/prototype/matches.mjs
  (function(undefined2) {
    var detect = "document" in this && "matches" in document.documentElement;
    if (detect) return;
    Element.prototype.matches = Element.prototype.webkitMatchesSelector || Element.prototype.oMatchesSelector || Element.prototype.msMatchesSelector || Element.prototype.mozMatchesSelector || function matches(selector) {
      var element = this;
      var elements = (element.document || element.ownerDocument).querySelectorAll(selector);
      var index = 0;
      while (elements[index] && elements[index] !== element) {
        ++index;
      }
      return !!elements[index];
    };
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Element/prototype/closest.mjs
  (function(undefined2) {
    var detect = "document" in this && "closest" in document.documentElement;
    if (detect) return;
    Element.prototype.closest = function closest(selector) {
      var node = this;
      while (node) {
        if (node.matches(selector)) return node;
        else node = "SVGElement" in window && node instanceof SVGElement ? node.parentNode : node.parentElement;
      }
      return null;
    };
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Window.mjs
  (function(undefined2) {
    var detect = "Window" in this;
    if (detect) return;
    if (typeof WorkerGlobalScope === "undefined" && typeof importScripts !== "function") {
      (function(global2) {
        if (global2.constructor) {
          global2.Window = global2.constructor;
        } else {
          (global2.Window = global2.constructor = new Function("return function Window() {}")()).prototype = this;
        }
      })(this);
    }
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Event.mjs
  (function(undefined2) {
    var detect = function(global2) {
      if (!("Event" in global2)) return false;
      if (typeof global2.Event === "function") return true;
      try {
        new Event("click");
        return true;
      } catch (e) {
        return false;
      }
    }(this);
    if (detect) return;
    (function() {
      var unlistenableWindowEvents = {
        click: 1,
        dblclick: 1,
        keyup: 1,
        keypress: 1,
        keydown: 1,
        mousedown: 1,
        mouseup: 1,
        mousemove: 1,
        mouseover: 1,
        mouseenter: 1,
        mouseleave: 1,
        mouseout: 1,
        storage: 1,
        storagecommit: 1,
        textinput: 1
      };
      if (typeof document === "undefined" || typeof window === "undefined") return;
      function indexOf(array, element) {
        var index = -1, length = array.length;
        while (++index < length) {
          if (index in array && array[index] === element) {
            return index;
          }
        }
        return -1;
      }
      var existingProto = window.Event && window.Event.prototype || null;
      window.Event = Window.prototype.Event = function Event2(type, eventInitDict) {
        if (!type) {
          throw new Error("Not enough arguments");
        }
        var event;
        if ("createEvent" in document) {
          event = document.createEvent("Event");
          var bubbles = eventInitDict && eventInitDict.bubbles !== undefined2 ? eventInitDict.bubbles : false;
          var cancelable = eventInitDict && eventInitDict.cancelable !== undefined2 ? eventInitDict.cancelable : false;
          event.initEvent(type, bubbles, cancelable);
          return event;
        }
        event = document.createEventObject();
        event.type = type;
        event.bubbles = eventInitDict && eventInitDict.bubbles !== undefined2 ? eventInitDict.bubbles : false;
        event.cancelable = eventInitDict && eventInitDict.cancelable !== undefined2 ? eventInitDict.cancelable : false;
        return event;
      };
      if (existingProto) {
        Object.defineProperty(window.Event, "prototype", {
          configurable: false,
          enumerable: false,
          writable: true,
          value: existingProto
        });
      }
      if (!("createEvent" in document)) {
        window.addEventListener = Window.prototype.addEventListener = Document.prototype.addEventListener = Element.prototype.addEventListener = function addEventListener() {
          var element = this, type = arguments[0], listener = arguments[1];
          if (element === window && type in unlistenableWindowEvents) {
            throw new Error("In IE8 the event: " + type + " is not available on the window object. Please see https://github.com/Financial-Times/polyfill-service/issues/317 for more information.");
          }
          if (!element._events) {
            element._events = {};
          }
          if (!element._events[type]) {
            element._events[type] = function(event) {
              var list = element._events[event.type].list, events = list.slice(), index = -1, length = events.length, eventElement;
              event.preventDefault = function preventDefault() {
                if (event.cancelable !== false) {
                  event.returnValue = false;
                }
              };
              event.stopPropagation = function stopPropagation() {
                event.cancelBubble = true;
              };
              event.stopImmediatePropagation = function stopImmediatePropagation() {
                event.cancelBubble = true;
                event.cancelImmediate = true;
              };
              event.currentTarget = element;
              event.relatedTarget = event.fromElement || null;
              event.target = event.target || event.srcElement || element;
              event.timeStamp = (/* @__PURE__ */ new Date()).getTime();
              if (event.clientX) {
                event.pageX = event.clientX + document.documentElement.scrollLeft;
                event.pageY = event.clientY + document.documentElement.scrollTop;
              }
              while (++index < length && !event.cancelImmediate) {
                if (index in events) {
                  eventElement = events[index];
                  if (indexOf(list, eventElement) !== -1 && typeof eventElement === "function") {
                    eventElement.call(element, event);
                  }
                }
              }
            };
            element._events[type].list = [];
            if (element.attachEvent) {
              element.attachEvent("on" + type, element._events[type]);
            }
          }
          element._events[type].list.push(listener);
        };
        window.removeEventListener = Window.prototype.removeEventListener = Document.prototype.removeEventListener = Element.prototype.removeEventListener = function removeEventListener() {
          var element = this, type = arguments[0], listener = arguments[1], index;
          if (element._events && element._events[type] && element._events[type].list) {
            index = indexOf(element._events[type].list, listener);
            if (index !== -1) {
              element._events[type].list.splice(index, 1);
              if (!element._events[type].list.length) {
                if (element.detachEvent) {
                  element.detachEvent("on" + type, element._events[type]);
                }
                delete element._events[type];
              }
            }
          }
        };
        window.dispatchEvent = Window.prototype.dispatchEvent = Document.prototype.dispatchEvent = Element.prototype.dispatchEvent = function dispatchEvent(event) {
          if (!arguments.length) {
            throw new Error("Not enough arguments");
          }
          if (!event || typeof event.type !== "string") {
            throw new Error("DOM Events Exception 0");
          }
          var element = this, type = event.type;
          try {
            if (!event.bubbles) {
              event.cancelBubble = true;
              var cancelBubbleEvent = function(event2) {
                event2.cancelBubble = true;
                (element || window).detachEvent("on" + type, cancelBubbleEvent);
              };
              this.attachEvent("on" + type, cancelBubbleEvent);
            }
            this.fireEvent("on" + type, event);
          } catch (error) {
            event.target = element;
            do {
              event.currentTarget = element;
              if ("_events" in element && typeof element._events[type] === "function") {
                element._events[type].call(element, event);
              }
              if (typeof element["on" + type] === "function") {
                element["on" + type].call(element, event);
              }
              element = element.nodeType === 9 ? element.parentWindow : element.parentNode;
            } while (element && !event.cancelBubble);
          }
          return true;
        };
        document.attachEvent("onreadystatechange", function() {
          if (document.readyState === "complete") {
            document.dispatchEvent(new Event("DOMContentLoaded", {
              bubbles: true
            }));
          }
        });
      }
    })();
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Function/prototype/bind.mjs
  (function(undefined2) {
    var detect = "bind" in Function.prototype;
    if (detect) return;
    Object.defineProperty(Function.prototype, "bind", {
      value: function bind(that) {
        var $Array = Array;
        var $Object = Object;
        var ObjectPrototype = $Object.prototype;
        var ArrayPrototype = $Array.prototype;
        var Empty = function Empty2() {
        };
        var to_string = ObjectPrototype.toString;
        var hasToStringTag = typeof Symbol === "function" && typeof Symbol.toStringTag === "symbol";
        var isCallable;
        var fnToStr = Function.prototype.toString, tryFunctionObject = function tryFunctionObject2(value) {
          try {
            fnToStr.call(value);
            return true;
          } catch (e) {
            return false;
          }
        }, fnClass = "[object Function]", genClass = "[object GeneratorFunction]";
        isCallable = function isCallable2(value) {
          if (typeof value !== "function") {
            return false;
          }
          if (hasToStringTag) {
            return tryFunctionObject(value);
          }
          var strClass = to_string.call(value);
          return strClass === fnClass || strClass === genClass;
        };
        var array_slice = ArrayPrototype.slice;
        var array_concat = ArrayPrototype.concat;
        var array_push = ArrayPrototype.push;
        var max = Math.max;
        var target = this;
        if (!isCallable(target)) {
          throw new TypeError("Function.prototype.bind called on incompatible " + target);
        }
        var args = array_slice.call(arguments, 1);
        var bound;
        var binder = function() {
          if (this instanceof bound) {
            var result = target.apply(
              this,
              array_concat.call(args, array_slice.call(arguments))
            );
            if ($Object(result) === result) {
              return result;
            }
            return this;
          } else {
            return target.apply(
              that,
              array_concat.call(args, array_slice.call(arguments))
            );
          }
        };
        var boundLength = max(0, target.length - args.length);
        var boundArgs = [];
        for (var i = 0; i < boundLength; i++) {
          array_push.call(boundArgs, "$" + i);
        }
        bound = Function("binder", "return function (" + boundArgs.join(",") + "){ return binder.apply(this, arguments); }")(binder);
        if (target.prototype) {
          Empty.prototype = target.prototype;
          bound.prototype = new Empty();
          Empty.prototype = null;
        }
        return bound;
      }
    });
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/components/accordion/accordion.mjs
  var ACCORDION_TRANSLATIONS = {
    hideAllSections: "Hide all sections",
    hideSection: "Hide",
    hideSectionAriaLabel: "Hide this section",
    showAllSections: "Show all sections",
    showSection: "Show",
    showSectionAriaLabel: "Show this section"
  };
  function Accordion($module, config) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    this.$module = $module;
    var defaultConfig = {
      i18n: ACCORDION_TRANSLATIONS,
      rememberExpanded: true
    };
    this.config = mergeConfigs(
      defaultConfig,
      config || {},
      normaliseDataset($module.dataset)
    );
    this.i18n = new I18n(extractConfigByNamespace(this.config, "i18n"));
    this.controlsClass = "govuk-accordion__controls";
    this.showAllClass = "govuk-accordion__show-all";
    this.showAllTextClass = "govuk-accordion__show-all-text";
    this.sectionClass = "govuk-accordion__section";
    this.sectionExpandedClass = "govuk-accordion__section--expanded";
    this.sectionButtonClass = "govuk-accordion__section-button";
    this.sectionHeaderClass = "govuk-accordion__section-header";
    this.sectionHeadingClass = "govuk-accordion__section-heading";
    this.sectionHeadingDividerClass = "govuk-accordion__section-heading-divider";
    this.sectionHeadingTextClass = "govuk-accordion__section-heading-text";
    this.sectionHeadingTextFocusClass = "govuk-accordion__section-heading-text-focus";
    this.sectionShowHideToggleClass = "govuk-accordion__section-toggle";
    this.sectionShowHideToggleFocusClass = "govuk-accordion__section-toggle-focus";
    this.sectionShowHideTextClass = "govuk-accordion__section-toggle-text";
    this.upChevronIconClass = "govuk-accordion-nav__chevron";
    this.downChevronIconClass = "govuk-accordion-nav__chevron--down";
    this.sectionSummaryClass = "govuk-accordion__section-summary";
    this.sectionSummaryFocusClass = "govuk-accordion__section-summary-focus";
    this.sectionContentClass = "govuk-accordion__section-content";
    var $sections = this.$module.querySelectorAll("." + this.sectionClass);
    if (!$sections.length) {
      return this;
    }
    this.$sections = $sections;
    this.browserSupportsSessionStorage = helper.checkForSessionStorage();
    this.$showAllButton = null;
    this.$showAllIcon = null;
    this.$showAllText = null;
  }
  Accordion.prototype.init = function() {
    if (!this.$module || !this.$sections) {
      return;
    }
    this.initControls();
    this.initSectionHeaders();
    var areAllSectionsOpen = this.checkIfAllSectionsOpen();
    this.updateShowAllButton(areAllSectionsOpen);
  };
  Accordion.prototype.initControls = function() {
    this.$showAllButton = document.createElement("button");
    this.$showAllButton.setAttribute("type", "button");
    this.$showAllButton.setAttribute("class", this.showAllClass);
    this.$showAllButton.setAttribute("aria-expanded", "false");
    this.$showAllIcon = document.createElement("span");
    this.$showAllIcon.classList.add(this.upChevronIconClass);
    this.$showAllButton.appendChild(this.$showAllIcon);
    var $accordionControls = document.createElement("div");
    $accordionControls.setAttribute("class", this.controlsClass);
    $accordionControls.appendChild(this.$showAllButton);
    this.$module.insertBefore($accordionControls, this.$module.firstChild);
    this.$showAllText = document.createElement("span");
    this.$showAllText.classList.add(this.showAllTextClass);
    this.$showAllButton.appendChild(this.$showAllText);
    this.$showAllButton.addEventListener("click", this.onShowOrHideAllToggle.bind(this));
    if ("onbeforematch" in document) {
      document.addEventListener("beforematch", this.onBeforeMatch.bind(this));
    }
  };
  Accordion.prototype.initSectionHeaders = function() {
    var $component = this;
    var $sections = this.$sections;
    nodeListForEach($sections, function($section, i) {
      var $header = $section.querySelector("." + $component.sectionHeaderClass);
      if (!$header) {
        return;
      }
      $component.constructHeaderMarkup($header, i);
      $component.setExpanded($component.isExpanded($section), $section);
      $header.addEventListener("click", $component.onSectionToggle.bind($component, $section));
      $component.setInitialState($section);
    });
  };
  Accordion.prototype.constructHeaderMarkup = function($header, index) {
    var $span = $header.querySelector("." + this.sectionButtonClass);
    var $heading = $header.querySelector("." + this.sectionHeadingClass);
    var $summary = $header.querySelector("." + this.sectionSummaryClass);
    if (!$span || !$heading) {
      return;
    }
    var $button = document.createElement("button");
    $button.setAttribute("type", "button");
    $button.setAttribute("aria-controls", this.$module.id + "-content-" + (index + 1).toString());
    for (var i = 0; i < $span.attributes.length; i++) {
      var attr = $span.attributes.item(i);
      if (attr.nodeName !== "id") {
        $button.setAttribute(attr.nodeName, attr.nodeValue);
      }
    }
    var $headingText = document.createElement("span");
    $headingText.classList.add(this.sectionHeadingTextClass);
    $headingText.id = $span.id;
    var $headingTextFocus = document.createElement("span");
    $headingTextFocus.classList.add(this.sectionHeadingTextFocusClass);
    $headingText.appendChild($headingTextFocus);
    $headingTextFocus.innerHTML = $span.innerHTML;
    var $showHideToggle = document.createElement("span");
    $showHideToggle.classList.add(this.sectionShowHideToggleClass);
    $showHideToggle.setAttribute("data-nosnippet", "");
    var $showHideToggleFocus = document.createElement("span");
    $showHideToggleFocus.classList.add(this.sectionShowHideToggleFocusClass);
    $showHideToggle.appendChild($showHideToggleFocus);
    var $showHideText = document.createElement("span");
    var $showHideIcon = document.createElement("span");
    $showHideIcon.classList.add(this.upChevronIconClass);
    $showHideToggleFocus.appendChild($showHideIcon);
    $showHideText.classList.add(this.sectionShowHideTextClass);
    $showHideToggleFocus.appendChild($showHideText);
    $button.appendChild($headingText);
    $button.appendChild(this.getButtonPunctuationEl());
    if ($summary) {
      var $summarySpan = document.createElement("span");
      var $summarySpanFocus = document.createElement("span");
      $summarySpanFocus.classList.add(this.sectionSummaryFocusClass);
      $summarySpan.appendChild($summarySpanFocus);
      for (var j = 0, l = $summary.attributes.length; j < l; ++j) {
        var nodeName = $summary.attributes.item(j).nodeName;
        var nodeValue = $summary.attributes.item(j).nodeValue;
        $summarySpan.setAttribute(nodeName, nodeValue);
      }
      $summarySpanFocus.innerHTML = $summary.innerHTML;
      $summary.parentNode.replaceChild($summarySpan, $summary);
      $button.appendChild($summarySpan);
      $button.appendChild(this.getButtonPunctuationEl());
    }
    $button.appendChild($showHideToggle);
    $heading.removeChild($span);
    $heading.appendChild($button);
  };
  Accordion.prototype.onBeforeMatch = function(event) {
    var $fragment = event.target;
    if (!($fragment instanceof Element)) {
      return;
    }
    var $section = $fragment.closest("." + this.sectionClass);
    if ($section) {
      this.setExpanded(true, $section);
    }
  };
  Accordion.prototype.onSectionToggle = function($section) {
    var expanded = this.isExpanded($section);
    this.setExpanded(!expanded, $section);
    this.storeState($section);
  };
  Accordion.prototype.onShowOrHideAllToggle = function() {
    var $component = this;
    var $sections = this.$sections;
    var nowExpanded = !this.checkIfAllSectionsOpen();
    nodeListForEach($sections, function($section) {
      $component.setExpanded(nowExpanded, $section);
      $component.storeState($section);
    });
    $component.updateShowAllButton(nowExpanded);
  };
  Accordion.prototype.setExpanded = function(expanded, $section) {
    var $showHideIcon = $section.querySelector("." + this.upChevronIconClass);
    var $showHideText = $section.querySelector("." + this.sectionShowHideTextClass);
    var $button = $section.querySelector("." + this.sectionButtonClass);
    var $content = $section.querySelector("." + this.sectionContentClass);
    if (!$showHideIcon || !($showHideText instanceof HTMLElement) || !$button || !$content) {
      return;
    }
    var newButtonText = expanded ? this.i18n.t("hideSection") : this.i18n.t("showSection");
    $showHideText.innerText = newButtonText;
    $button.setAttribute("aria-expanded", expanded.toString());
    var ariaLabelParts = [];
    var $headingText = $section.querySelector("." + this.sectionHeadingTextClass);
    if ($headingText instanceof HTMLElement) {
      ariaLabelParts.push($headingText.innerText.trim());
    }
    var $summary = $section.querySelector("." + this.sectionSummaryClass);
    if ($summary instanceof HTMLElement) {
      ariaLabelParts.push($summary.innerText.trim());
    }
    var ariaLabelMessage = expanded ? this.i18n.t("hideSectionAriaLabel") : this.i18n.t("showSectionAriaLabel");
    ariaLabelParts.push(ariaLabelMessage);
    $button.setAttribute("aria-label", ariaLabelParts.join(" , "));
    if (expanded) {
      $content.removeAttribute("hidden");
      $section.classList.add(this.sectionExpandedClass);
      $showHideIcon.classList.remove(this.downChevronIconClass);
    } else {
      $content.setAttribute("hidden", "until-found");
      $section.classList.remove(this.sectionExpandedClass);
      $showHideIcon.classList.add(this.downChevronIconClass);
    }
    var areAllSectionsOpen = this.checkIfAllSectionsOpen();
    this.updateShowAllButton(areAllSectionsOpen);
  };
  Accordion.prototype.isExpanded = function($section) {
    return $section.classList.contains(this.sectionExpandedClass);
  };
  Accordion.prototype.checkIfAllSectionsOpen = function() {
    var sectionsCount = this.$sections.length;
    var expandedSectionCount = this.$module.querySelectorAll("." + this.sectionExpandedClass).length;
    var areAllSectionsOpen = sectionsCount === expandedSectionCount;
    return areAllSectionsOpen;
  };
  Accordion.prototype.updateShowAllButton = function(expanded) {
    var newButtonText = expanded ? this.i18n.t("hideAllSections") : this.i18n.t("showAllSections");
    this.$showAllButton.setAttribute("aria-expanded", expanded.toString());
    this.$showAllText.innerText = newButtonText;
    if (expanded) {
      this.$showAllIcon.classList.remove(this.downChevronIconClass);
    } else {
      this.$showAllIcon.classList.add(this.downChevronIconClass);
    }
  };
  var helper = {
    /**
     * Check for `window.sessionStorage`, and that it actually works.
     *
     * @returns {boolean} True if session storage is available
     */
    checkForSessionStorage: function() {
      var testString = "this is the test string";
      var result;
      try {
        window.sessionStorage.setItem(testString, testString);
        result = window.sessionStorage.getItem(testString) === testString.toString();
        window.sessionStorage.removeItem(testString);
        return result;
      } catch (exception) {
        return false;
      }
    }
  };
  Accordion.prototype.storeState = function($section) {
    if (this.browserSupportsSessionStorage && this.config.rememberExpanded) {
      var $button = $section.querySelector("." + this.sectionButtonClass);
      if ($button) {
        var contentId = $button.getAttribute("aria-controls");
        var contentState = $button.getAttribute("aria-expanded");
        if (contentId && contentState) {
          window.sessionStorage.setItem(contentId, contentState);
        }
      }
    }
  };
  Accordion.prototype.setInitialState = function($section) {
    if (this.browserSupportsSessionStorage && this.config.rememberExpanded) {
      var $button = $section.querySelector("." + this.sectionButtonClass);
      if ($button) {
        var contentId = $button.getAttribute("aria-controls");
        var contentState = contentId ? window.sessionStorage.getItem(contentId) : null;
        if (contentState !== null) {
          this.setExpanded(contentState === "true", $section);
        }
      }
    }
  };
  Accordion.prototype.getButtonPunctuationEl = function() {
    var $punctuationEl = document.createElement("span");
    $punctuationEl.classList.add("govuk-visually-hidden", this.sectionHeadingDividerClass);
    $punctuationEl.innerHTML = ", ";
    return $punctuationEl;
  };
  var accordion_default = Accordion;

  // node_modules/govuk-frontend/govuk-esm/components/button/button.mjs
  var KEY_SPACE = 32;
  var DEBOUNCE_TIMEOUT_IN_SECONDS = 1;
  function Button($module, config) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    this.$module = $module;
    this.debounceFormSubmitTimer = null;
    var defaultConfig = {
      preventDoubleClick: false
    };
    this.config = mergeConfigs(
      defaultConfig,
      config || {},
      normaliseDataset($module.dataset)
    );
  }
  Button.prototype.init = function() {
    if (!this.$module) {
      return;
    }
    this.$module.addEventListener("keydown", this.handleKeyDown);
    this.$module.addEventListener("click", this.debounce.bind(this));
  };
  Button.prototype.handleKeyDown = function(event) {
    var $target = event.target;
    if (event.keyCode !== KEY_SPACE) {
      return;
    }
    if ($target instanceof HTMLElement && $target.getAttribute("role") === "button") {
      event.preventDefault();
      $target.click();
    }
  };
  Button.prototype.debounce = function(event) {
    if (!this.config.preventDoubleClick) {
      return;
    }
    if (this.debounceFormSubmitTimer) {
      event.preventDefault();
      return false;
    }
    this.debounceFormSubmitTimer = setTimeout(function() {
      this.debounceFormSubmitTimer = null;
    }.bind(this), DEBOUNCE_TIMEOUT_IN_SECONDS * 1e3);
  };
  var button_default = Button;

  // node_modules/govuk-frontend/govuk-esm/common/closest-attribute-value.mjs
  function closestAttributeValue($element, attributeName) {
    var $closestElementWithAttribute = $element.closest("[" + attributeName + "]");
    return $closestElementWithAttribute ? $closestElementWithAttribute.getAttribute(attributeName) : null;
  }

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Date/now.mjs
  (function(undefined2) {
    var detect = "Date" in self && "now" in self.Date && "getTime" in self.Date.prototype;
    if (detect) return;
    Date.now = function() {
      return (/* @__PURE__ */ new Date()).getTime();
    };
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/components/character-count/character-count.mjs
  var CHARACTER_COUNT_TRANSLATIONS = {
    // Characters
    charactersUnderLimit: {
      one: "You have %{count} character remaining",
      other: "You have %{count} characters remaining"
    },
    charactersAtLimit: "You have 0 characters remaining",
    charactersOverLimit: {
      one: "You have %{count} character too many",
      other: "You have %{count} characters too many"
    },
    // Words
    wordsUnderLimit: {
      one: "You have %{count} word remaining",
      other: "You have %{count} words remaining"
    },
    wordsAtLimit: "You have 0 words remaining",
    wordsOverLimit: {
      one: "You have %{count} word too many",
      other: "You have %{count} words too many"
    },
    textareaDescription: {
      other: ""
    }
  };
  function CharacterCount($module, config) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    var $textarea = $module.querySelector(".govuk-js-character-count");
    if (!($textarea instanceof HTMLTextAreaElement || $textarea instanceof HTMLInputElement)) {
      return this;
    }
    var defaultConfig = {
      threshold: 0,
      i18n: CHARACTER_COUNT_TRANSLATIONS
    };
    var datasetConfig = normaliseDataset($module.dataset);
    var configOverrides = {};
    if ("maxwords" in datasetConfig || "maxlength" in datasetConfig) {
      configOverrides = {
        maxlength: false,
        maxwords: false
      };
    }
    this.config = mergeConfigs(
      defaultConfig,
      config || {},
      configOverrides,
      datasetConfig
    );
    this.i18n = new I18n(extractConfigByNamespace(this.config, "i18n"), {
      // Read the fallback if necessary rather than have it set in the defaults
      locale: closestAttributeValue($module, "lang")
    });
    this.maxLength = Infinity;
    if ("maxwords" in this.config && this.config.maxwords) {
      this.maxLength = this.config.maxwords;
    } else if ("maxlength" in this.config && this.config.maxlength) {
      this.maxLength = this.config.maxlength;
    } else {
      return;
    }
    this.$module = $module;
    this.$textarea = $textarea;
    this.$visibleCountMessage = null;
    this.$screenReaderCountMessage = null;
    this.lastInputTimestamp = null;
    this.lastInputValue = "";
    this.valueChecker = null;
  }
  CharacterCount.prototype.init = function() {
    if (!this.$module || !this.$textarea) {
      return;
    }
    var $textarea = this.$textarea;
    var $textareaDescription = document.getElementById($textarea.id + "-info");
    if (!$textareaDescription) {
      return;
    }
    if ($textareaDescription.innerText.match(/^\s*$/)) {
      $textareaDescription.innerText = this.i18n.t("textareaDescription", { count: this.maxLength });
    }
    $textarea.insertAdjacentElement("afterend", $textareaDescription);
    var $screenReaderCountMessage = document.createElement("div");
    $screenReaderCountMessage.className = "govuk-character-count__sr-status govuk-visually-hidden";
    $screenReaderCountMessage.setAttribute("aria-live", "polite");
    this.$screenReaderCountMessage = $screenReaderCountMessage;
    $textareaDescription.insertAdjacentElement("afterend", $screenReaderCountMessage);
    var $visibleCountMessage = document.createElement("div");
    $visibleCountMessage.className = $textareaDescription.className;
    $visibleCountMessage.classList.add("govuk-character-count__status");
    $visibleCountMessage.setAttribute("aria-hidden", "true");
    this.$visibleCountMessage = $visibleCountMessage;
    $textareaDescription.insertAdjacentElement("afterend", $visibleCountMessage);
    $textareaDescription.classList.add("govuk-visually-hidden");
    $textarea.removeAttribute("maxlength");
    this.bindChangeEvents();
    window.addEventListener(
      "onpageshow" in window ? "pageshow" : "DOMContentLoaded",
      this.updateCountMessage.bind(this)
    );
    this.updateCountMessage();
  };
  CharacterCount.prototype.bindChangeEvents = function() {
    var $textarea = this.$textarea;
    $textarea.addEventListener("keyup", this.handleKeyUp.bind(this));
    $textarea.addEventListener("focus", this.handleFocus.bind(this));
    $textarea.addEventListener("blur", this.handleBlur.bind(this));
  };
  CharacterCount.prototype.handleKeyUp = function() {
    this.updateVisibleCountMessage();
    this.lastInputTimestamp = Date.now();
  };
  CharacterCount.prototype.handleFocus = function() {
    this.valueChecker = setInterval(function() {
      if (!this.lastInputTimestamp || Date.now() - 500 >= this.lastInputTimestamp) {
        this.updateIfValueChanged();
      }
    }.bind(this), 1e3);
  };
  CharacterCount.prototype.handleBlur = function() {
    clearInterval(this.valueChecker);
  };
  CharacterCount.prototype.updateIfValueChanged = function() {
    if (this.$textarea.value !== this.lastInputValue) {
      this.lastInputValue = this.$textarea.value;
      this.updateCountMessage();
    }
  };
  CharacterCount.prototype.updateCountMessage = function() {
    this.updateVisibleCountMessage();
    this.updateScreenReaderCountMessage();
  };
  CharacterCount.prototype.updateVisibleCountMessage = function() {
    var $textarea = this.$textarea;
    var $visibleCountMessage = this.$visibleCountMessage;
    var remainingNumber = this.maxLength - this.count($textarea.value);
    if (this.isOverThreshold()) {
      $visibleCountMessage.classList.remove("govuk-character-count__message--disabled");
    } else {
      $visibleCountMessage.classList.add("govuk-character-count__message--disabled");
    }
    if (remainingNumber < 0) {
      $textarea.classList.add("govuk-textarea--error");
      $visibleCountMessage.classList.remove("govuk-hint");
      $visibleCountMessage.classList.add("govuk-error-message");
    } else {
      $textarea.classList.remove("govuk-textarea--error");
      $visibleCountMessage.classList.remove("govuk-error-message");
      $visibleCountMessage.classList.add("govuk-hint");
    }
    $visibleCountMessage.innerText = this.getCountMessage();
  };
  CharacterCount.prototype.updateScreenReaderCountMessage = function() {
    var $screenReaderCountMessage = this.$screenReaderCountMessage;
    if (this.isOverThreshold()) {
      $screenReaderCountMessage.removeAttribute("aria-hidden");
    } else {
      $screenReaderCountMessage.setAttribute("aria-hidden", "true");
    }
    $screenReaderCountMessage.innerText = this.getCountMessage();
  };
  CharacterCount.prototype.count = function(text) {
    if ("maxwords" in this.config && this.config.maxwords) {
      var tokens = text.match(/\S+/g) || [];
      return tokens.length;
    } else {
      return text.length;
    }
  };
  CharacterCount.prototype.getCountMessage = function() {
    var remainingNumber = this.maxLength - this.count(this.$textarea.value);
    var countType = "maxwords" in this.config && this.config.maxwords ? "words" : "characters";
    return this.formatCountMessage(remainingNumber, countType);
  };
  CharacterCount.prototype.formatCountMessage = function(remainingNumber, countType) {
    if (remainingNumber === 0) {
      return this.i18n.t(countType + "AtLimit");
    }
    var translationKeySuffix = remainingNumber < 0 ? "OverLimit" : "UnderLimit";
    return this.i18n.t(countType + translationKeySuffix, { count: Math.abs(remainingNumber) });
  };
  CharacterCount.prototype.isOverThreshold = function() {
    if (!this.config.threshold) {
      return true;
    }
    var $textarea = this.$textarea;
    var currentLength = this.count($textarea.value);
    var maxLength = this.maxLength;
    var thresholdValue = maxLength * this.config.threshold / 100;
    return thresholdValue <= currentLength;
  };
  var character_count_default = CharacterCount;

  // node_modules/govuk-frontend/govuk-esm/components/checkboxes/checkboxes.mjs
  function Checkboxes($module) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    var $inputs = $module.querySelectorAll('input[type="checkbox"]');
    if (!$inputs.length) {
      return this;
    }
    this.$module = $module;
    this.$inputs = $inputs;
  }
  Checkboxes.prototype.init = function() {
    if (!this.$module || !this.$inputs) {
      return;
    }
    var $module = this.$module;
    var $inputs = this.$inputs;
    nodeListForEach($inputs, function($input) {
      var targetId = $input.getAttribute("data-aria-controls");
      if (!targetId || !document.getElementById(targetId)) {
        return;
      }
      $input.setAttribute("aria-controls", targetId);
      $input.removeAttribute("data-aria-controls");
    });
    window.addEventListener(
      "onpageshow" in window ? "pageshow" : "DOMContentLoaded",
      this.syncAllConditionalReveals.bind(this)
    );
    this.syncAllConditionalReveals();
    $module.addEventListener("click", this.handleClick.bind(this));
  };
  Checkboxes.prototype.syncAllConditionalReveals = function() {
    nodeListForEach(this.$inputs, this.syncConditionalRevealWithInputState.bind(this));
  };
  Checkboxes.prototype.syncConditionalRevealWithInputState = function($input) {
    var targetId = $input.getAttribute("aria-controls");
    if (!targetId) {
      return;
    }
    var $target = document.getElementById(targetId);
    if ($target && $target.classList.contains("govuk-checkboxes__conditional")) {
      var inputIsChecked = $input.checked;
      $input.setAttribute("aria-expanded", inputIsChecked.toString());
      $target.classList.toggle("govuk-checkboxes__conditional--hidden", !inputIsChecked);
    }
  };
  Checkboxes.prototype.unCheckAllInputsExcept = function($input) {
    var $component = this;
    var allInputsWithSameName = document.querySelectorAll(
      'input[type="checkbox"][name="' + $input.name + '"]'
    );
    nodeListForEach(allInputsWithSameName, function($inputWithSameName) {
      var hasSameFormOwner = $input.form === $inputWithSameName.form;
      if (hasSameFormOwner && $inputWithSameName !== $input) {
        $inputWithSameName.checked = false;
        $component.syncConditionalRevealWithInputState($inputWithSameName);
      }
    });
  };
  Checkboxes.prototype.unCheckExclusiveInputs = function($input) {
    var $component = this;
    var allInputsWithSameNameAndExclusiveBehaviour = document.querySelectorAll(
      'input[data-behaviour="exclusive"][type="checkbox"][name="' + $input.name + '"]'
    );
    nodeListForEach(allInputsWithSameNameAndExclusiveBehaviour, function($exclusiveInput) {
      var hasSameFormOwner = $input.form === $exclusiveInput.form;
      if (hasSameFormOwner) {
        $exclusiveInput.checked = false;
        $component.syncConditionalRevealWithInputState($exclusiveInput);
      }
    });
  };
  Checkboxes.prototype.handleClick = function(event) {
    var $clickedInput = event.target;
    if (!($clickedInput instanceof HTMLInputElement) || $clickedInput.type !== "checkbox") {
      return;
    }
    var hasAriaControls = $clickedInput.getAttribute("aria-controls");
    if (hasAriaControls) {
      this.syncConditionalRevealWithInputState($clickedInput);
    }
    if (!$clickedInput.checked) {
      return;
    }
    var hasBehaviourExclusive = $clickedInput.getAttribute("data-behaviour") === "exclusive";
    if (hasBehaviourExclusive) {
      this.unCheckAllInputsExcept($clickedInput);
    } else {
      this.unCheckExclusiveInputs($clickedInput);
    }
  };
  var checkboxes_default = Checkboxes;

  // node_modules/govuk-frontend/govuk-esm/components/details/details.mjs
  var KEY_ENTER = 13;
  var KEY_SPACE2 = 32;
  function Details($module) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    this.$module = $module;
    this.$summary = null;
    this.$content = null;
  }
  Details.prototype.init = function() {
    if (!this.$module) {
      return;
    }
    var hasNativeDetails = "HTMLDetailsElement" in window && this.$module instanceof HTMLDetailsElement;
    if (!hasNativeDetails) {
      this.polyfillDetails();
    }
  };
  Details.prototype.polyfillDetails = function() {
    var $module = this.$module;
    var $summary = this.$summary = $module.getElementsByTagName("summary").item(0);
    var $content = this.$content = $module.getElementsByTagName("div").item(0);
    if (!$summary || !$content) {
      return;
    }
    if (!$content.id) {
      $content.id = "details-content-" + generateUniqueID();
    }
    $module.setAttribute("role", "group");
    $summary.setAttribute("role", "button");
    $summary.setAttribute("aria-controls", $content.id);
    $summary.tabIndex = 0;
    if (this.$module.hasAttribute("open")) {
      $summary.setAttribute("aria-expanded", "true");
    } else {
      $summary.setAttribute("aria-expanded", "false");
      $content.style.display = "none";
    }
    this.polyfillHandleInputs(this.polyfillSetAttributes.bind(this));
  };
  Details.prototype.polyfillSetAttributes = function() {
    if (this.$module.hasAttribute("open")) {
      this.$module.removeAttribute("open");
      this.$summary.setAttribute("aria-expanded", "false");
      this.$content.style.display = "none";
    } else {
      this.$module.setAttribute("open", "open");
      this.$summary.setAttribute("aria-expanded", "true");
      this.$content.style.display = "";
    }
    return true;
  };
  Details.prototype.polyfillHandleInputs = function(callback) {
    this.$summary.addEventListener("keypress", function(event) {
      var $target = event.target;
      if (event.keyCode === KEY_ENTER || event.keyCode === KEY_SPACE2) {
        if ($target instanceof HTMLElement && $target.nodeName.toLowerCase() === "summary") {
          event.preventDefault();
          if ($target.click) {
            $target.click();
          } else {
            callback(event);
          }
        }
      }
    });
    this.$summary.addEventListener("keyup", function(event) {
      var $target = event.target;
      if (event.keyCode === KEY_SPACE2) {
        if ($target instanceof HTMLElement && $target.nodeName.toLowerCase() === "summary") {
          event.preventDefault();
        }
      }
    });
    this.$summary.addEventListener("click", callback);
  };
  var details_default = Details;

  // node_modules/govuk-frontend/govuk-esm/components/error-summary/error-summary.mjs
  function ErrorSummary($module, config) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    this.$module = $module;
    var defaultConfig = {
      disableAutoFocus: false
    };
    this.config = mergeConfigs(
      defaultConfig,
      config || {},
      normaliseDataset($module.dataset)
    );
  }
  ErrorSummary.prototype.init = function() {
    if (!this.$module) {
      return;
    }
    var $module = this.$module;
    this.setFocus();
    $module.addEventListener("click", this.handleClick.bind(this));
  };
  ErrorSummary.prototype.setFocus = function() {
    var $module = this.$module;
    if (this.config.disableAutoFocus) {
      return;
    }
    $module.setAttribute("tabindex", "-1");
    $module.addEventListener("blur", function() {
      $module.removeAttribute("tabindex");
    });
    $module.focus();
  };
  ErrorSummary.prototype.handleClick = function(event) {
    var $target = event.target;
    if (this.focusTarget($target)) {
      event.preventDefault();
    }
  };
  ErrorSummary.prototype.focusTarget = function($target) {
    if (!($target instanceof HTMLAnchorElement)) {
      return false;
    }
    var inputId = this.getFragmentFromUrl($target.href);
    if (!inputId) {
      return false;
    }
    var $input = document.getElementById(inputId);
    if (!$input) {
      return false;
    }
    var $legendOrLabel = this.getAssociatedLegendOrLabel($input);
    if (!$legendOrLabel) {
      return false;
    }
    $legendOrLabel.scrollIntoView();
    $input.focus({ preventScroll: true });
    return true;
  };
  ErrorSummary.prototype.getFragmentFromUrl = function(url) {
    if (url.indexOf("#") === -1) {
      return void 0;
    }
    return url.split("#").pop();
  };
  ErrorSummary.prototype.getAssociatedLegendOrLabel = function($input) {
    var $fieldset = $input.closest("fieldset");
    if ($fieldset) {
      var $legends = $fieldset.getElementsByTagName("legend");
      if ($legends.length) {
        var $candidateLegend = $legends[0];
        if ($input instanceof HTMLInputElement && ($input.type === "checkbox" || $input.type === "radio")) {
          return $candidateLegend;
        }
        var legendTop = $candidateLegend.getBoundingClientRect().top;
        var inputRect = $input.getBoundingClientRect();
        if (inputRect.height && window.innerHeight) {
          var inputBottom = inputRect.top + inputRect.height;
          if (inputBottom - legendTop < window.innerHeight / 2) {
            return $candidateLegend;
          }
        }
      }
    }
    return document.querySelector("label[for='" + $input.getAttribute("id") + "']") || $input.closest("label");
  };
  var error_summary_default = ErrorSummary;

  // node_modules/govuk-frontend/govuk-esm/components/exit-this-page/exit-this-page.mjs
  var EXIT_THIS_PAGE_TRANSLATIONS = {
    activated: "Loading.",
    timedOut: "Exit this page expired.",
    pressTwoMoreTimes: "Shift, press 2 more times to exit.",
    pressOneMoreTime: "Shift, press 1 more time to exit."
  };
  function ExitThisPage($module, config) {
    var defaultConfig = {
      i18n: EXIT_THIS_PAGE_TRANSLATIONS
    };
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    var $button = $module.querySelector(".govuk-exit-this-page__button");
    if (!($button instanceof HTMLElement)) {
      return this;
    }
    this.config = mergeConfigs(
      defaultConfig,
      config || {},
      normaliseDataset($module.dataset)
    );
    this.i18n = new I18n(extractConfigByNamespace(this.config, "i18n"));
    this.$module = $module;
    this.$button = $button;
    this.$skiplinkButton = document.querySelector(".govuk-js-exit-this-page-skiplink");
    this.$updateSpan = null;
    this.$indicatorContainer = null;
    this.$overlay = null;
    this.keypressCounter = 0;
    this.lastKeyWasModified = false;
    this.timeoutTime = 5e3;
    this.keypressTimeoutId = null;
    this.timeoutMessageId = null;
  }
  ExitThisPage.prototype.initUpdateSpan = function() {
    this.$updateSpan = document.createElement("span");
    this.$updateSpan.setAttribute("role", "status");
    this.$updateSpan.className = "govuk-visually-hidden";
    this.$module.appendChild(this.$updateSpan);
  };
  ExitThisPage.prototype.initButtonClickHandler = function() {
    this.$button.addEventListener("click", this.handleClick.bind(this));
    if (this.$skiplinkButton) {
      this.$skiplinkButton.addEventListener("click", this.handleClick.bind(this));
    }
  };
  ExitThisPage.prototype.buildIndicator = function() {
    this.$indicatorContainer = document.createElement("div");
    this.$indicatorContainer.className = "govuk-exit-this-page__indicator";
    this.$indicatorContainer.setAttribute("aria-hidden", "true");
    for (var i = 0; i < 3; i++) {
      var $indicator = document.createElement("div");
      $indicator.className = "govuk-exit-this-page__indicator-light";
      this.$indicatorContainer.appendChild($indicator);
    }
    this.$button.appendChild(this.$indicatorContainer);
  };
  ExitThisPage.prototype.updateIndicator = function() {
    if (this.keypressCounter > 0) {
      this.$indicatorContainer.classList.add("govuk-exit-this-page__indicator--visible");
    } else {
      this.$indicatorContainer.classList.remove("govuk-exit-this-page__indicator--visible");
    }
    var $indicators = this.$indicatorContainer.querySelectorAll(
      ".govuk-exit-this-page__indicator-light"
    );
    nodeListForEach($indicators, function($indicator, index) {
      $indicator.classList.toggle(
        "govuk-exit-this-page__indicator-light--on",
        index < this.keypressCounter
      );
    }.bind(this));
  };
  ExitThisPage.prototype.exitPage = function() {
    this.$updateSpan.innerText = "";
    document.body.classList.add("govuk-exit-this-page-hide-content");
    this.$overlay = document.createElement("div");
    this.$overlay.className = "govuk-exit-this-page-overlay";
    this.$overlay.setAttribute("role", "alert");
    document.body.appendChild(this.$overlay);
    this.$overlay.innerText = this.i18n.t("activated");
    window.location.href = this.$button.getAttribute("href");
  };
  ExitThisPage.prototype.handleClick = function(event) {
    event.preventDefault();
    this.exitPage();
  };
  ExitThisPage.prototype.handleKeypress = function(event) {
    if ((event.key === "Shift" || event.keyCode === 16 || event.which === 16) && !this.lastKeyWasModified) {
      this.keypressCounter += 1;
      this.updateIndicator();
      if (this.timeoutMessageId !== null) {
        clearTimeout(this.timeoutMessageId);
        this.timeoutMessageId = null;
      }
      if (this.keypressCounter >= 3) {
        this.keypressCounter = 0;
        if (this.keypressTimeoutId !== null) {
          clearTimeout(this.keypressTimeoutId);
          this.keypressTimeoutId = null;
        }
        this.exitPage();
      } else {
        if (this.keypressCounter === 1) {
          this.$updateSpan.innerText = this.i18n.t("pressTwoMoreTimes");
        } else {
          this.$updateSpan.innerText = this.i18n.t("pressOneMoreTime");
        }
      }
      this.setKeypressTimer();
    } else if (this.keypressTimeoutId !== null) {
      this.resetKeypressTimer();
    }
    this.lastKeyWasModified = event.shiftKey;
  };
  ExitThisPage.prototype.setKeypressTimer = function() {
    clearTimeout(this.keypressTimeoutId);
    this.keypressTimeoutId = setTimeout(
      this.resetKeypressTimer.bind(this),
      this.timeoutTime
    );
  };
  ExitThisPage.prototype.resetKeypressTimer = function() {
    clearTimeout(this.keypressTimeoutId);
    this.keypressTimeoutId = null;
    this.keypressCounter = 0;
    this.$updateSpan.innerText = this.i18n.t("timedOut");
    this.timeoutMessageId = setTimeout(function() {
      this.$updateSpan.innerText = "";
    }.bind(this), this.timeoutTime);
    this.updateIndicator();
  };
  ExitThisPage.prototype.resetPage = function() {
    document.body.classList.remove("govuk-exit-this-page-hide-content");
    if (this.$overlay) {
      this.$overlay.remove();
      this.$overlay = null;
    }
    this.$updateSpan.setAttribute("role", "status");
    this.$updateSpan.innerText = "";
    this.updateIndicator();
    if (this.keypressTimeoutId) {
      clearTimeout(this.keypressTimeoutId);
    }
    if (this.timeoutMessageId) {
      clearTimeout(this.timeoutMessageId);
    }
  };
  ExitThisPage.prototype.init = function() {
    this.buildIndicator();
    this.initUpdateSpan();
    this.initButtonClickHandler();
    if (!("govukFrontendExitThisPageKeypress" in document.body.dataset)) {
      document.addEventListener("keyup", this.handleKeypress.bind(this), true);
      document.body.dataset.govukFrontendExitThisPageKeypress = "true";
    }
    window.addEventListener(
      "onpageshow" in window ? "pageshow" : "DOMContentLoaded",
      this.resetPage.bind(this)
    );
  };
  var exit_this_page_default = ExitThisPage;

  // node_modules/govuk-frontend/govuk-esm/components/header/header.mjs
  function Header($module) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    this.$module = $module;
    this.$menuButton = $module.querySelector(".govuk-js-header-toggle");
    this.$menu = this.$menuButton && $module.querySelector(
      "#" + this.$menuButton.getAttribute("aria-controls")
    );
    this.menuIsOpen = false;
    this.mql = null;
  }
  Header.prototype.init = function() {
    if (!this.$module || !this.$menuButton || !this.$menu) {
      return;
    }
    if ("matchMedia" in window) {
      this.mql = window.matchMedia("(min-width: 48.0625em)");
      if ("addEventListener" in this.mql) {
        this.mql.addEventListener("change", this.syncState.bind(this));
      } else {
        this.mql.addListener(this.syncState.bind(this));
      }
      this.syncState();
      this.$menuButton.addEventListener("click", this.handleMenuButtonClick.bind(this));
    } else {
      this.$menuButton.setAttribute("hidden", "");
    }
  };
  Header.prototype.syncState = function() {
    if (this.mql.matches) {
      this.$menu.removeAttribute("hidden");
      this.$menuButton.setAttribute("hidden", "");
    } else {
      this.$menuButton.removeAttribute("hidden");
      this.$menuButton.setAttribute("aria-expanded", this.menuIsOpen.toString());
      if (this.menuIsOpen) {
        this.$menu.removeAttribute("hidden");
      } else {
        this.$menu.setAttribute("hidden", "");
      }
    }
  };
  Header.prototype.handleMenuButtonClick = function() {
    this.menuIsOpen = !this.menuIsOpen;
    this.syncState();
  };
  var header_default = Header;

  // node_modules/govuk-frontend/govuk-esm/components/notification-banner/notification-banner.mjs
  function NotificationBanner($module, config) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    this.$module = $module;
    var defaultConfig = {
      disableAutoFocus: false
    };
    this.config = mergeConfigs(
      defaultConfig,
      config || {},
      normaliseDataset($module.dataset)
    );
  }
  NotificationBanner.prototype.init = function() {
    if (!this.$module) {
      return;
    }
    this.setFocus();
  };
  NotificationBanner.prototype.setFocus = function() {
    var $module = this.$module;
    if (this.config.disableAutoFocus) {
      return;
    }
    if ($module.getAttribute("role") !== "alert") {
      return;
    }
    if (!$module.getAttribute("tabindex")) {
      $module.setAttribute("tabindex", "-1");
      $module.addEventListener("blur", function() {
        $module.removeAttribute("tabindex");
      });
    }
    $module.focus();
  };
  var notification_banner_default = NotificationBanner;

  // node_modules/govuk-frontend/govuk-esm/components/radios/radios.mjs
  function Radios($module) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    var $inputs = $module.querySelectorAll('input[type="radio"]');
    if (!$inputs.length) {
      return this;
    }
    this.$module = $module;
    this.$inputs = $inputs;
  }
  Radios.prototype.init = function() {
    if (!this.$module || !this.$inputs) {
      return;
    }
    var $module = this.$module;
    var $inputs = this.$inputs;
    nodeListForEach($inputs, function($input) {
      var targetId = $input.getAttribute("data-aria-controls");
      if (!targetId || !document.getElementById(targetId)) {
        return;
      }
      $input.setAttribute("aria-controls", targetId);
      $input.removeAttribute("data-aria-controls");
    });
    window.addEventListener(
      "onpageshow" in window ? "pageshow" : "DOMContentLoaded",
      this.syncAllConditionalReveals.bind(this)
    );
    this.syncAllConditionalReveals();
    $module.addEventListener("click", this.handleClick.bind(this));
  };
  Radios.prototype.syncAllConditionalReveals = function() {
    nodeListForEach(this.$inputs, this.syncConditionalRevealWithInputState.bind(this));
  };
  Radios.prototype.syncConditionalRevealWithInputState = function($input) {
    var targetId = $input.getAttribute("aria-controls");
    if (!targetId) {
      return;
    }
    var $target = document.getElementById(targetId);
    if ($target && $target.classList.contains("govuk-radios__conditional")) {
      var inputIsChecked = $input.checked;
      $input.setAttribute("aria-expanded", inputIsChecked.toString());
      $target.classList.toggle("govuk-radios__conditional--hidden", !inputIsChecked);
    }
  };
  Radios.prototype.handleClick = function(event) {
    var $component = this;
    var $clickedInput = event.target;
    if (!($clickedInput instanceof HTMLInputElement) || $clickedInput.type !== "radio") {
      return;
    }
    var $allInputs = document.querySelectorAll('input[type="radio"][aria-controls]');
    var $clickedInputForm = $clickedInput.form;
    var $clickedInputName = $clickedInput.name;
    nodeListForEach($allInputs, function($input) {
      var hasSameFormOwner = $input.form === $clickedInputForm;
      var hasSameName = $input.name === $clickedInputName;
      if (hasSameName && hasSameFormOwner) {
        $component.syncConditionalRevealWithInputState($input);
      }
    });
  };
  var radios_default = Radios;

  // node_modules/govuk-frontend/govuk-esm/components/skip-link/skip-link.mjs
  function SkipLink($module) {
    if (!($module instanceof HTMLAnchorElement)) {
      return this;
    }
    this.$module = $module;
    this.$linkedElement = null;
    this.linkedElementListener = false;
  }
  SkipLink.prototype.init = function() {
    if (!this.$module) {
      return;
    }
    var $linkedElement = this.getLinkedElement();
    if (!$linkedElement) {
      return;
    }
    this.$linkedElement = $linkedElement;
    this.$module.addEventListener("click", this.focusLinkedElement.bind(this));
  };
  SkipLink.prototype.getLinkedElement = function() {
    var linkedElementId = this.getFragmentFromUrl();
    if (!linkedElementId) {
      return null;
    }
    return document.getElementById(linkedElementId);
  };
  SkipLink.prototype.focusLinkedElement = function() {
    var $linkedElement = this.$linkedElement;
    if (!$linkedElement.getAttribute("tabindex")) {
      $linkedElement.setAttribute("tabindex", "-1");
      $linkedElement.classList.add("govuk-skip-link-focused-element");
      if (!this.linkedElementListener) {
        this.$linkedElement.addEventListener("blur", this.removeFocusProperties.bind(this));
        this.linkedElementListener = true;
      }
    }
    $linkedElement.focus();
  };
  SkipLink.prototype.removeFocusProperties = function() {
    this.$linkedElement.removeAttribute("tabindex");
    this.$linkedElement.classList.remove("govuk-skip-link-focused-element");
  };
  SkipLink.prototype.getFragmentFromUrl = function() {
    if (!this.$module.hash) {
      return;
    }
    return this.$module.hash.split("#").pop();
  };
  var skip_link_default = SkipLink;

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Element/prototype/nextElementSibling.mjs
  (function(undefined2) {
    var detect = "document" in this && "nextElementSibling" in document.documentElement;
    if (detect) return;
    Object.defineProperty(Element.prototype, "nextElementSibling", {
      get: function() {
        var el = this.nextSibling;
        while (el && el.nodeType !== 1) {
          el = el.nextSibling;
        }
        return el;
      }
    });
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/vendor/polyfills/Element/prototype/previousElementSibling.mjs
  (function(undefined2) {
    var detect = "document" in this && "previousElementSibling" in document.documentElement;
    if (detect) return;
    Object.defineProperty(Element.prototype, "previousElementSibling", {
      get: function() {
        var el = this.previousSibling;
        while (el && el.nodeType !== 1) {
          el = el.previousSibling;
        }
        return el;
      }
    });
  }).call("object" === typeof window && window || "object" === typeof self && self || "object" === typeof global && global || {});

  // node_modules/govuk-frontend/govuk-esm/components/tabs/tabs.mjs
  function Tabs($module) {
    if (!($module instanceof HTMLElement)) {
      return this;
    }
    var $tabs = $module.querySelectorAll("a.govuk-tabs__tab");
    if (!$tabs.length) {
      return this;
    }
    this.$module = $module;
    this.$tabs = $tabs;
    this.keys = { left: 37, right: 39, up: 38, down: 40 };
    this.jsHiddenClass = "govuk-tabs__panel--hidden";
    this.boundTabClick = this.onTabClick.bind(this);
    this.boundTabKeydown = this.onTabKeydown.bind(this);
    this.boundOnHashChange = this.onHashChange.bind(this);
    this.changingHash = false;
  }
  Tabs.prototype.init = function() {
    if (!this.$module || !this.$tabs) {
      return;
    }
    if (typeof window.matchMedia === "function") {
      this.setupResponsiveChecks();
    } else {
      this.setup();
    }
  };
  Tabs.prototype.setupResponsiveChecks = function() {
    this.mql = window.matchMedia("(min-width: 40.0625em)");
    this.mql.addListener(this.checkMode.bind(this));
    this.checkMode();
  };
  Tabs.prototype.checkMode = function() {
    if (this.mql.matches) {
      this.setup();
    } else {
      this.teardown();
    }
  };
  Tabs.prototype.setup = function() {
    var $component = this;
    var $module = this.$module;
    var $tabs = this.$tabs;
    var $tabList = $module.querySelector(".govuk-tabs__list");
    var $tabListItems = $module.querySelectorAll(".govuk-tabs__list-item");
    if (!$tabs || !$tabList || !$tabListItems) {
      return;
    }
    $tabList.setAttribute("role", "tablist");
    nodeListForEach($tabListItems, function($item) {
      $item.setAttribute("role", "presentation");
    });
    nodeListForEach($tabs, function($tab) {
      $component.setAttributes($tab);
      $tab.addEventListener("click", $component.boundTabClick, true);
      $tab.addEventListener("keydown", $component.boundTabKeydown, true);
      $component.hideTab($tab);
    });
    var $activeTab = this.getTab(window.location.hash) || this.$tabs[0];
    if (!$activeTab) {
      return;
    }
    this.showTab($activeTab);
    window.addEventListener("hashchange", this.boundOnHashChange, true);
  };
  Tabs.prototype.teardown = function() {
    var $component = this;
    var $module = this.$module;
    var $tabs = this.$tabs;
    var $tabList = $module.querySelector(".govuk-tabs__list");
    var $tabListItems = $module.querySelectorAll("a.govuk-tabs__list-item");
    if (!$tabs || !$tabList || !$tabListItems) {
      return;
    }
    $tabList.removeAttribute("role");
    nodeListForEach($tabListItems, function($item) {
      $item.removeAttribute("role");
    });
    nodeListForEach($tabs, function($tab) {
      $tab.removeEventListener("click", $component.boundTabClick, true);
      $tab.removeEventListener("keydown", $component.boundTabKeydown, true);
      $component.unsetAttributes($tab);
    });
    window.removeEventListener("hashchange", this.boundOnHashChange, true);
  };
  Tabs.prototype.onHashChange = function() {
    var hash = window.location.hash;
    var $tabWithHash = this.getTab(hash);
    if (!$tabWithHash) {
      return;
    }
    if (this.changingHash) {
      this.changingHash = false;
      return;
    }
    var $previousTab = this.getCurrentTab();
    if (!$previousTab) {
      return;
    }
    this.hideTab($previousTab);
    this.showTab($tabWithHash);
    $tabWithHash.focus();
  };
  Tabs.prototype.hideTab = function($tab) {
    this.unhighlightTab($tab);
    this.hidePanel($tab);
  };
  Tabs.prototype.showTab = function($tab) {
    this.highlightTab($tab);
    this.showPanel($tab);
  };
  Tabs.prototype.getTab = function(hash) {
    return this.$module.querySelector('a.govuk-tabs__tab[href="' + hash + '"]');
  };
  Tabs.prototype.setAttributes = function($tab) {
    var panelId = this.getHref($tab).slice(1);
    $tab.setAttribute("id", "tab_" + panelId);
    $tab.setAttribute("role", "tab");
    $tab.setAttribute("aria-controls", panelId);
    $tab.setAttribute("aria-selected", "false");
    $tab.setAttribute("tabindex", "-1");
    var $panel = this.getPanel($tab);
    if (!$panel) {
      return;
    }
    $panel.setAttribute("role", "tabpanel");
    $panel.setAttribute("aria-labelledby", $tab.id);
    $panel.classList.add(this.jsHiddenClass);
  };
  Tabs.prototype.unsetAttributes = function($tab) {
    $tab.removeAttribute("id");
    $tab.removeAttribute("role");
    $tab.removeAttribute("aria-controls");
    $tab.removeAttribute("aria-selected");
    $tab.removeAttribute("tabindex");
    var $panel = this.getPanel($tab);
    if (!$panel) {
      return;
    }
    $panel.removeAttribute("role");
    $panel.removeAttribute("aria-labelledby");
    $panel.classList.remove(this.jsHiddenClass);
  };
  Tabs.prototype.onTabClick = function(event) {
    var $currentTab = this.getCurrentTab();
    var $nextTab = event.currentTarget;
    if (!$currentTab || !($nextTab instanceof HTMLAnchorElement)) {
      return;
    }
    event.preventDefault();
    this.hideTab($currentTab);
    this.showTab($nextTab);
    this.createHistoryEntry($nextTab);
  };
  Tabs.prototype.createHistoryEntry = function($tab) {
    var $panel = this.getPanel($tab);
    if (!$panel) {
      return;
    }
    var panelId = $panel.id;
    $panel.id = "";
    this.changingHash = true;
    window.location.hash = this.getHref($tab).slice(1);
    $panel.id = panelId;
  };
  Tabs.prototype.onTabKeydown = function(event) {
    switch (event.keyCode) {
      case this.keys.left:
      case this.keys.up:
        this.activatePreviousTab();
        event.preventDefault();
        break;
      case this.keys.right:
      case this.keys.down:
        this.activateNextTab();
        event.preventDefault();
        break;
    }
  };
  Tabs.prototype.activateNextTab = function() {
    var $currentTab = this.getCurrentTab();
    if (!$currentTab || !$currentTab.parentElement) {
      return;
    }
    var $nextTabListItem = $currentTab.parentElement.nextElementSibling;
    if (!$nextTabListItem) {
      return;
    }
    var $nextTab = $nextTabListItem.querySelector("a.govuk-tabs__tab");
    if (!$nextTab) {
      return;
    }
    this.hideTab($currentTab);
    this.showTab($nextTab);
    $nextTab.focus();
    this.createHistoryEntry($nextTab);
  };
  Tabs.prototype.activatePreviousTab = function() {
    var $currentTab = this.getCurrentTab();
    if (!$currentTab || !$currentTab.parentElement) {
      return;
    }
    var $previousTabListItem = $currentTab.parentElement.previousElementSibling;
    if (!$previousTabListItem) {
      return;
    }
    var $previousTab = $previousTabListItem.querySelector("a.govuk-tabs__tab");
    if (!$previousTab) {
      return;
    }
    this.hideTab($currentTab);
    this.showTab($previousTab);
    $previousTab.focus();
    this.createHistoryEntry($previousTab);
  };
  Tabs.prototype.getPanel = function($tab) {
    return this.$module.querySelector(this.getHref($tab));
  };
  Tabs.prototype.showPanel = function($tab) {
    var $panel = this.getPanel($tab);
    if (!$panel) {
      return;
    }
    $panel.classList.remove(this.jsHiddenClass);
  };
  Tabs.prototype.hidePanel = function($tab) {
    var $panel = this.getPanel($tab);
    if (!$panel) {
      return;
    }
    $panel.classList.add(this.jsHiddenClass);
  };
  Tabs.prototype.unhighlightTab = function($tab) {
    if (!$tab.parentElement) {
      return;
    }
    $tab.setAttribute("aria-selected", "false");
    $tab.parentElement.classList.remove("govuk-tabs__list-item--selected");
    $tab.setAttribute("tabindex", "-1");
  };
  Tabs.prototype.highlightTab = function($tab) {
    if (!$tab.parentElement) {
      return;
    }
    $tab.setAttribute("aria-selected", "true");
    $tab.parentElement.classList.add("govuk-tabs__list-item--selected");
    $tab.setAttribute("tabindex", "0");
  };
  Tabs.prototype.getCurrentTab = function() {
    return this.$module.querySelector(".govuk-tabs__list-item--selected a.govuk-tabs__tab");
  };
  Tabs.prototype.getHref = function($tab) {
    var href = $tab.getAttribute("href");
    var hash = href.slice(href.indexOf("#"), href.length);
    return hash;
  };
  var tabs_default = Tabs;

  // node_modules/govuk-frontend/govuk-esm/all.mjs
  function initAll(config) {
    config = typeof config !== "undefined" ? config : {};
    var $scope = config.scope instanceof HTMLElement ? config.scope : document;
    var $accordions = $scope.querySelectorAll('[data-module="govuk-accordion"]');
    nodeListForEach($accordions, function($accordion) {
      new accordion_default($accordion, config.accordion).init();
    });
    var $buttons = $scope.querySelectorAll('[data-module="govuk-button"]');
    nodeListForEach($buttons, function($button) {
      new button_default($button, config.button).init();
    });
    var $characterCounts = $scope.querySelectorAll('[data-module="govuk-character-count"]');
    nodeListForEach($characterCounts, function($characterCount) {
      new character_count_default($characterCount, config.characterCount).init();
    });
    var $checkboxes = $scope.querySelectorAll('[data-module="govuk-checkboxes"]');
    nodeListForEach($checkboxes, function($checkbox) {
      new checkboxes_default($checkbox).init();
    });
    var $details = $scope.querySelectorAll('[data-module="govuk-details"]');
    nodeListForEach($details, function($detail) {
      new details_default($detail).init();
    });
    var $errorSummary = $scope.querySelector('[data-module="govuk-error-summary"]');
    if ($errorSummary) {
      new error_summary_default($errorSummary, config.errorSummary).init();
    }
    var $exitThisPageButtons = $scope.querySelectorAll('[data-module="govuk-exit-this-page"]');
    nodeListForEach($exitThisPageButtons, function($button) {
      new exit_this_page_default($button, config.exitThisPage).init();
    });
    var $header = $scope.querySelector('[data-module="govuk-header"]');
    if ($header) {
      new header_default($header).init();
    }
    var $notificationBanners = $scope.querySelectorAll('[data-module="govuk-notification-banner"]');
    nodeListForEach($notificationBanners, function($notificationBanner) {
      new notification_banner_default($notificationBanner, config.notificationBanner).init();
    });
    var $radios = $scope.querySelectorAll('[data-module="govuk-radios"]');
    nodeListForEach($radios, function($radio) {
      new radios_default($radio).init();
    });
    var $skipLink = $scope.querySelector('[data-module="govuk-skip-link"]');
    if ($skipLink) {
      new skip_link_default($skipLink).init();
    }
    var $tabs = $scope.querySelectorAll('[data-module="govuk-tabs"]');
    nodeListForEach($tabs, function($tabs2) {
      new tabs_default($tabs2).init();
    });
  }

  // assets/js/refresh.js
  setInterval(function() {
    window.location.reload();
  }, 5e3);

  // assets/js/app.js
  initAll(window.eprprnrepexp.initializationPayload);
})();
//# sourceMappingURL=app.bundle.js.map
