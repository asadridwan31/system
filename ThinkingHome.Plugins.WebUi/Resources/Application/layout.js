var lib = require('lib');
var layoutTemplate = require('static/web-ui/layout.tpl');
var errorTemplate = '<h1><i class="fa fa-times-circle fa-fw text-danger"></i> {{title}}</h1>' +
    '<p class="lead">{{message}}</p>';

var ErrorView = lib.marionette.View.extend({
    className: 'th-error',
    template: lib.handlebars.compile(errorTemplate),
    templateContext: function() {
        return {
            title: this.getOption('title'),
            message: this.getOption('message')
        }
    }
});

var LayoutView = lib.marionette.View.extend({
    el: 'body',

    template: lib.handlebars.compile(layoutTemplate),

    regions: {
        content: '.js-content'
    },

    events: {
        'click .js-toggler': function() { this.toggleMenu(); },
        'click .js-nav-link': function() { this.toggleMenu(false); }
    },

    toggleMenu: function(enabled) {
        this.$('.js-menu').toggleClass('show', enabled);
    }
});

var Layout = lib.common.ApplicationBlock.extend({
    initialize: function() {
        this.view = new LayoutView();
    },
    render: function() {
        this.view.render();
    },
    onBeforeDestroy: function () {
        this.view.destroy();
    },

    // api
    setContentView: function(view) {
        this.view.showChildView('content', view);
    },

    showErrorPage: function(title, message) {
        var errorView = new ErrorView({
            title: title,
            message: message
        });

        this.view.showChildView('content', errorView);
    },

    loading: function() {

    }
});

module.exports = Layout;
