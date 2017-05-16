'use strict';

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

(function ($) {
    'use strict';

    var Boarddropdown = function () {
        function Boarddropdown(element, options) {
            _classCallCheck(this, Boarddropdown);

            this.$element = element;
            this.options = $.extend({}, Boarddropdown.defaults, this.$element.data, options);
            this._init();
            Foundation.registerPlugin(this, 'Boarddropdown');
        }

        _createClass(Boarddropdown, [{
            key: '_init',
            value: function _init() {
                var id = this.$element.attr('id');
                this.$toggler = $('[data-toggle="' + id + '"]');
                this._events();
            }
        }, {
            key: '_events',
            value: function _events() {
                this.$element.on("toggle.zf.trigger", this.toggle.bind(this));
            }
        }, {
            key: 'destroy',
            value: function destroy() {
                this.$element.off("toggle.zf.trigger").hide();
                Foundation.unregisterPlugin(this);
            }
        }, {
            key: 'toggle',
            value: function toggle() {
                if (this.$element.hasClass('is-open')) {
                    this.close();
                } else {
                    this.open();
                }
            }
        }, {
            key: 'open',
            value: function open() {
                if (this.$element.hasClass('is-open')) {
                    return;
                }
                this.$element.addClass('is-open');
                if (this.options.autoClose) {
                    this.addBodyHandler();
                }
            }
        }, {
            key: 'addBodyHandler',
            value: function addBodyHandler() {
                var $bodyItems = $(document.body).not(this.$element);
                var _this = this;
                $bodyItems.off('click.zf.boarddropdown').on('click.zf.boarddropdown', function (ev) {
                    // if we clicked on the element or on its descendant
                    if (_this.$element.find(ev.target).length) {
                        return;
                    }
                    // if we clicked on the toggler
                    if (_this.$toggler.is(ev.target) || _this.$toggler.find(ev.target).length) {
                        return;
                    }

                    _this.close();
                    $bodyItems.off('click.zf.boarddropdown');
                });
            }
        }, {
            key: 'close',
            value: function close() {
                if (!this.$element.hasClass('is-open')) {
                    return;
                }
                this.$element.removeClass('is-open');
            }
        }]);

        return Boarddropdown;
    }();

    Boarddropdown.defaults = {
        autoClose: true
    };

    Foundation.plugin(Boarddropdown, 'Boarddropdown');
})(jQuery);