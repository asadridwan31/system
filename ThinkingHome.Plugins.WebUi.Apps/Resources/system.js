var lang = require('lang!static/web-ui/apps/lang.json');
var appsSection = require('/static/web-ui/apps/common.js');

module.exports = appsSection.extend({
    title: lang.get('System settings'),
    url: '/api/web-ui/apps/system'
});
