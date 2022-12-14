/*
 *  webui popover plugin  - v1.2.0
 *  A lightWeight popover plugin with jquery ,enchance the  popover plugin of bootstrap with some awesome new features. It works well with bootstrap ,but bootstrap is not necessary!
 *  https://github.com/sandywalker/webui-popover
 *
 *  Made by Sandy Duan
 *  Under MIT License
 */
! function (a, b, c) {
  "use strict";

  function d(b, c) {
    this.$element = a(b), c && ("string" === a.type(c.delay) || "number" === a.type(c.delay)) && (c.delay = {
      show: c.delay,
      hide: c.delay
    }), this.options = a.extend({}, h, c), this._defaults = h, this._name = e, this._targetclick = !1, this.init(), i.push(this.$element)
  }
  var e = "webuiPopover",
    f = "webui-popover",
    g = "webui.popover",
    h = {
      placement: "auto",
      width: "auto",
      height: "auto",
      trigger: "click",
      style: "",
      delay: {
        show: null,
        hide: null
      },
      async: {
        before: null,
        success: null
      },
      cache: !0,
      multi: !1,
      arrow: !0,
      title: "",
      content: "",
      closeable: !1,
      padding: !0,
      url: "",
      type: "html",
      animation: null,
      template: '<div class="webui-popover"><div class="arrow"></div><div class="webui-popover-inner"><a href="#" class="close">x</a><h3 class="webui-popover-title"></h3><div class="webui-popover-content"><i class="icon-refresh"></i> <p>&nbsp;</p></div></div></div>',
      backdrop: !1,
      dismissible: !0,
      onShow: null,
      onHide: null,
      abortXHR: !0,
      autoHide: !1,
      offsetTop: 0,
      offsetLeft: 0
    },
    i = [],
    j = a('<div class="webui-popover-backdrop"></div>'),
    k = 0,
    l = !1,
    m = -2e3,
    n = a(c),
    o = function (a, b) {
      return isNaN(a) ? b || 0 : Number(a)
    };
  d.prototype = {
    init: function () {
      "click" === this.getTrigger() ? this.$element.off("click touchend").on("click touchend", a.proxy(this.toggle, this)) : "hover" === this.getTrigger() && this.$element.off("mouseenter mouseleave click").on("mouseenter", a.proxy(this.mouseenterHandler, this)).on("mouseleave", a.proxy(this.mouseleaveHandler, this)), this._poped = !1, this._inited = !0, this._opened = !1, this._idSeed = k, this.options.backdrop && j.appendTo(c.body).hide(), k++, "sticky" === this.getTrigger() && this.show()
    },
    destroy: function () {
      for (var a = -1, b = 0; b < i.length; b++)
        if (i[b] === this.$element) {
          a = b;
          break
        }
      i.splice(a, 1), this.hide(), this.$element.data("plugin_" + e, null), "click" === this.getTrigger() ? this.$element.off("click") : "hover" === this.getTrigger() && this.$element.off("mouseenter mouseleave"), this.$target && this.$target.remove()
    },
    hide: function (b, c) {
      if ((b || "sticky" !== this.getTrigger()) && this._opened) {
        c && (c.preventDefault(), c.stopPropagation()), this.xhr && this.options.abortXHR === !0 && (this.xhr.abort(), this.xhr = null);
        var d = a.Event("hide." + g);
        if (this.$element.trigger(d, [this.$target]), this.$target) {
          this.$target.removeClass("in").addClass(this.getHideAnimation());
          var e = this;
          setTimeout(function () {
            e.$target.hide()
          }, 300)
        }
        this.options.backdrop && j.hide(), this._opened = !1, this.$element.trigger("hidden." + g, [this.$target]), this.options.onHide && this.options.onHide(this.$target)
      }
    },
    resetAutoHide: function () {
      var a = this,
        b = a.getAutoHide();
      b && (a.autoHideHandler && clearTimeout(a.autoHideHandler), a.autoHideHandler = setTimeout(function () {
        a.hide()
      }, b))
    },
    toggle: function (a) {
      a && (a.preventDefault(), a.stopPropagation()), this[this.getTarget().hasClass("in") ? "hide" : "show"]()
    },
    hideAll: function () {
      for (var a = 0; a < i.length; a++) i[a].webuiPopover("hide");
      n.trigger("hiddenAll." + g)
    },
    show: function () {
      var a = this.getTarget().removeClass().addClass(f).addClass(this._customTargetClass);
      this.options.multi || this.hideAll(), this._opened || (this.getCache() && this._poped && "" !== this.content || (this.content = "", this.setTitle(this.getTitle()), this.options.closeable || a.find(".close").off("click").remove(), this.isAsync() ? this.setContentASync(this.options.content) : this.setContent(this.getContent()), a.show()), this.displayContent(), this.options.onShow && this.options.onShow(a), this.bindBodyEvents(), this.options.backdrop && j.show(), this._opened = !0, this.resetAutoHide())
    },
    displayContent: function () {
      var b = this.getElementPosition(),
        d = this.getTarget().removeClass().addClass(f).addClass(this._customTargetClass),
        e = this.getContentElement(),
        h = d[0].offsetWidth,
        i = d[0].offsetHeight,
        j = "bottom",
        k = a.Event("show." + g);
      this.$element.trigger(k, [d]), "auto" !== this.options.width && d.width(this.options.width), "auto" !== this.options.height && e.height(this.options.height), this.options.style && this.$target.addClass(f + "-" + this.options.style), this.options.arrow || d.find(".arrow").remove(), d.detach().css({
        top: m,
        left: m,
        display: "block"
      }), this.getAnimation() && d.addClass(this.getAnimation()), d.appendTo(c.body), j = this.getPlacement(b), this.$element.trigger("added." + g), this.initTargetEvents(), this.options.padding || ("auto" !== this.options.height && e.css("height", e.outerHeight()), this.$target.addClass("webui-no-padding")), h = d[0].offsetWidth, i = d[0].offsetHeight;
      var l = this.getTargetPositin(b, j, h, i);
      if (this.$target.css(l.position).addClass(j).addClass("in"), "iframe" === this.options.type) {
        var n = d.find("iframe");
        n.width(d.width()).height(n.parent().height())
      }
      if (this.options.arrow || this.$target.css({
          margin: 0
        }), this.options.arrow) {
        var o = this.$target.find(".arrow");
        o.removeAttr("style"), l.arrowOffset && (-1 === l.arrowOffset.left || -1 === l.arrowOffset.top ? o.hide() : o.css(l.arrowOffset))
      }
      this._poped = !0, this.$element.trigger("shown." + g, [this.$target])
    },
    isTargetLoaded: function () {
      return 0 === this.getTarget().find("i.glyphicon-refresh").length
    },
    getTriggerElement: function () {
      return this.$element
    },
    getTarget: function () {
      if (!this.$target) {
        var b = e + this._idSeed;
        this.$target = a(this.options.template).attr("id", b).data("trigger-element", this.getTriggerElement()), this._customTargetClass = this.$target.attr("class") !== f ? this.$target.attr("class") : null, this.getTriggerElement().attr("data-target", b)
      }
      return this.$target
    },
    getTitleElement: function () {
      return this.getTarget().find("." + f + "-title")
    },
    getContentElement: function () {
      return this.$contentElement || (this.$contentElement = this.getTarget().find("." + f + "-content"), console.log(this.$contentElement), this.$contentElement.show()), this.$contentElement
    },
    getTitle: function () {
      return this.$element.attr("data-title") || this.options.title || this.$element.attr("title")
    },
    getUrl: function () {
      return this.$element.attr("data-url") || this.options.url
    },
    getAutoHide: function () {
      return this.$element.attr("data-auto-hide") || this.options.autoHide
    },
    getOffsetTop: function () {
      return o(this.$element.attr("data-offset-top")) || this.options.offsetTop
    },
    getOffsetLeft: function () {
      return o(this.$element.attr("data-offset-left")) || this.options.offsetLeft
    },
    getCache: function () {
      var a = this.$element.attr("data-cache");
      if ("undefined" != typeof a) switch (a.toLowerCase()) {
        case "true":
        case "yes":
        case "1":
          return !0;
        case "false":
        case "no":
        case "0":
          return !1
      }
      return this.options.cache
    },
    getTrigger: function () {
      return this.$element.attr("data-trigger") || this.options.trigger
    },
    getDelayShow: function () {
      var a = this.$element.attr("data-delay-show");
      return "undefined" != typeof a ? a : 0 === this.options.delay.show ? 0 : this.options.delay.show || 100
    },
    getHideDelay: function () {
      var a = this.$element.attr("data-delay-hide");
      return "undefined" != typeof a ? a : 0 === this.options.delay.hide ? 0 : this.options.delay.hide || 100
    },
    getAnimation: function () {
      var a = this.$element.attr("data-animation");
      return a || this.options.animation
    },
    getHideAnimation: function () {
      var a = this.getAnimation();
      return a ? a + "-out" : "out"
    },
    setTitle: function (a) {
      var b = this.getTitleElement();
      a ? b.html(a) : b.remove()
    },
    hasContent: function () {
      return this.getContent()
    },
    getContent: function () {
      if (this.getUrl()) switch (this.options.type) {
        case "iframe":
          this.content = a('<iframe frameborder="0"></iframe>').attr("src", this.getUrl());
          break;
        case "html":
          try {
            this.content = a(this.getUrl()), this.content.is(":visible") || this.content.show()
          } catch (b) {
            throw new Error("Unable to get popover content. Invalid selector specified.")
          }
      } else if (!this.content) {
        var c = "";
        if (c = a.isFunction(this.options.content) ? this.options.content.apply(this.$element[0], [this]) : this.options.content, this.content = this.$element.attr("data-content") || c, !this.content) {
          var d = this.$element.children();
          d && d.hasClass(f + "-content") && (this.content = d)
        }
      }
      return this.content
    },
    setContent: function (a) {
      var b = this.getTarget(),
        c = this.getContentElement();
      "string" == typeof a ? c.html(a) : a instanceof jQuery && (a.removeClass(f + "-content"), c.html(""), a.appendTo(c)), this.$target = b
    },
    isAsync: function () {
      return "async" === this.options.type
    },
    setContentASync: function (b) {
      var c = this;
      this.xhr || (this.xhr = a.ajax({
        url: this.getUrl(),
        type: "GET",
        cache: this.getCache(),
        beforeSend: function (a) {
          c.options.async.before && c.options.async.before(c, a)
        },
        success: function (d) {
          c.bindBodyEvents(), c.content = b && a.isFunction(b) ? b.apply(c.$element[0], [d]) : d, c.setContent(c.content);
          var e = c.getContentElement();
          e.removeAttr("style"), c.displayContent(), c.options.async.success && c.options.async.success(c, d)
        },
        complete: function () {
          c.xhr = null
        }
      }))
    },
    bindBodyEvents: function () {
      this.options.dismissible && "click" === this.getTrigger() && !l && (n.off("keyup.webui-popover").on("keyup.webui-popover", a.proxy(this.escapeHandler, this)), n.off("click.webui-popover touchend.webui-popover").on("click.webui-popover touchend.webui-popover", a.proxy(this.bodyClickHandler, this)))
    },
    mouseenterHandler: function () {
      var a = this;
      a._timeout && clearTimeout(a._timeout), a._enterTimeout = setTimeout(function () {
        a.getTarget().is(":visible") || a.show()
      }, this.getDelayShow())
    },
    mouseleaveHandler: function () {
      var a = this;
      clearTimeout(a._enterTimeout), a._timeout = setTimeout(function () {
        a.hide()
      }, this.getHideDelay())
    },
    escapeHandler: function (a) {
      27 === a.keyCode && this.hideAll()
    },
    bodyClickHandler: function () {
      l = !0, "click" === this.getTrigger() && (this._targetclick ? this._targetclick = !1 : this.hideAll())
    },
    targetClickHandler: function () {
      this._targetclick = !0
    },
    initTargetEvents: function () {
      "hover" === this.getTrigger() && this.$target.off("mouseenter mouseleave").on("mouseenter", a.proxy(this.mouseenterHandler, this)).on("mouseleave", a.proxy(this.mouseleaveHandler, this)), this.$target.find(".close").off("click").on("click", a.proxy(this.hide, this, !0)), this.$target.off("click.webui-popover").on("click.webui-popover", a.proxy(this.targetClickHandler, this))
    },
    getPlacement: function (a) {
      var b, d = c.documentElement,
        e = c.body,
        f = d.clientWidth,
        g = d.clientHeight,
        h = Math.max(e.scrollTop, d.scrollTop),
        i = Math.max(e.scrollLeft, d.scrollLeft),
        j = Math.max(0, a.left - i),
        k = Math.max(0, a.top - h);
      b = "function" == typeof this.options.placement ? this.options.placement.call(this, this.getTarget()[0], this.$element[0]) : this.$element.data("placement") || this.options.placement;
      var l = "horizontal" === b,
        m = "vertical" === b,
        n = "auto" === b || l || m;
      return n ? b = f / 3 > j ? g / 3 > k ? l ? "right-bottom" : "bottom-right" : 2 * g / 3 > k ? m ? g / 2 >= k ? "bottom-right" : "top-right" : "right" : l ? "right-top" : "top-right" : 2 * f / 3 > j ? g / 3 > k ? l ? f / 2 >= j ? "right-bottom" : "left-bottom" : "bottom" : 2 * g / 3 > k ? l ? f / 2 >= j ? "right" : "left" : g / 2 >= k ? "bottom" : "top" : l ? f / 2 >= j ? "right-top" : "left-top" : "top" : g / 3 > k ? l ? "left-bottom" : "bottom-left" : 2 * g / 3 > k ? m ? g / 2 >= k ? "bottom-left" : "top-left" : "left" : l ? "left-top" : "top-left" : "auto-top" === b ? b = f / 3 > j ? "top-right" : 2 * f / 3 > j ? "top" : "top-left" : "auto-bottom" === b ? b = f / 3 > j ? "bottom-right" : 2 * f / 3 > j ? "bottom" : "bottom-left" : "auto-left" === b ? b = g / 3 > k ? "left-top" : 2 * g / 3 > k ? "left" : "left-bottom" : "auto-right" === b && (b = g / 3 > k ? "right-top" : 2 * g / 3 > k ? "right" : "right-bottom"), b
    },
    getElementPosition: function () {
      return a.extend({}, this.$element.offset(), {
        width: this.$element[0].offsetWidth,
        height: this.$element[0].offsetHeight
      })
    },
    getTargetPositin: function (a, b, d, e) {
      var f = a,
        g = c.documentElement,
        h = c.body,
        i = g.clientWidth,
        j = g.clientHeight,
        k = this.$element.outerWidth(),
        l = this.$element.outerHeight(),
        n = Math.max(h.scrollTop, g.scrollTop),
        o = Math.max(h.scrollLeft, g.scrollLeft),
        p = {},
        q = null,
        r = this.options.arrow ? 20 : 0,
        s = 10,
        t = r + s > k ? r : 0,
        u = r + s > l ? r : 0,
        v = 0,
        w = j + n,
        x = i + o,
        y = f.left + f.width / 2 - t > 0,
        z = f.left + f.width / 2 + t < x,
        A = f.top + f.height / 2 - u > 0,
        B = f.top + f.height / 2 + u < w;
      switch (b) {
        case "bottom":
          p = {
            top: f.top + f.height,
            left: f.left + f.width / 2 - d / 2
          };
          break;
        case "top":
          p = {
            top: f.top - e,
            left: f.left + f.width / 2 - d / 2
          };
          break;
        case "left":
          p = {
            top: f.top + f.height / 2 - e / 2,
            left: f.left - d
          };
          break;
        case "right":
          p = {
            top: f.top + f.height / 2 - e / 2,
            left: f.left + f.width
          };
          break;
        case "top-right":
          p = {
            top: f.top - e,
            left: y ? f.left - t : s
          }, q = {
            left: y ? Math.min(k, d) / 2 + t : m
          };
          break;
        case "top-left":
          v = z ? t : -s, p = {
            top: f.top - e,
            left: f.left - d + f.width + v
          }, q = {
            left: z ? d - Math.min(k, d) / 2 - t : m
          };
          break;
        case "bottom-right":
          p = {
            top: f.top + f.height,
            left: y ? f.left - t : s
          }, q = {
            left: y ? Math.min(k, d) / 2 + t : m
          };
          break;
        case "bottom-left":
          v = z ? t : -s, p = {
            top: f.top + f.height,
            left: f.left - d + f.width + v
          }, q = {
            left: z ? d - Math.min(k, d) / 2 - t : m
          };
          break;
        case "right-top":
          v = B ? u : -s, p = {
            top: f.top - e + f.height + v,
            left: f.left + f.width
          }, q = {
            top: B ? e - Math.min(l, e) / 2 - u : m
          };
          break;
        case "right-bottom":
          p = {
            top: A ? f.top - u : s,
            left: f.left + f.width
          }, q = {
            top: A ? Math.min(l, e) / 2 + u : m
          };
          break;
        case "left-top":
          v = B ? u : -s, p = {
            top: f.top - e + f.height + v,
            left: f.left - d
          }, q = {
            top: B ? e - Math.min(l, e) / 2 - u : m
          };
          break;
        case "left-bottom":
          p = {
            top: A ? f.top - u : s,
            left: f.left - d
          }, q = {
            top: A ? Math.min(l, e) / 2 + u : m
          }
      }
      return p.top += this.getOffsetTop(), p.left += this.getOffsetLeft(), {
        position: p,
        arrowOffset: q
      }
    }
  }, a.fn[e] = function (b, c) {
    var f = [],
      g = this.each(function () {
        var g = a.data(this, "plugin_" + e);
        g ? "destroy" === b ? g.destroy() : "string" == typeof b && f.push(g[b]()) : (b ? "string" == typeof b ? "destroy" !== b && (c || (g = new d(this, null), f.push(g[b]()))) : "object" == typeof b && (g = new d(this, b)) : g = new d(this, null), a.data(this, "plugin_" + e, g))
      });
    return f.length ? f : g
  }
}(jQuery, window, document);