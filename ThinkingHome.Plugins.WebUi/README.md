*ThinkingHome.Plugins.WebUi*

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/ThinkingHome.Plugins.WebUi.svg)](https://www.nuget.org/packages/ThinkingHome.Plugins.WebUi)

# WebUiPlugin

Реализует инфраструктуру веб-интерфейса системы.

Технически, веб-интерфейс представляет собой модульное одностраничное приложение на основе [marionette.js](https://marionettejs.com). Загрузка модулей происходит по мере необходимости с помощью [systemjs](https://github.com/systemjs/systemjs). Также в интерфейсе доступны [Twitter Bootstrap](https://v4-alpha.getbootstrap.com) и [Font Awesome](http://fontawesome.io).

В плагине WebUiPlugin реализованы:

- общая разметка страницы (навигационное меню и область для контента),
- загрузка разделов интерфейса с сервера по требованию и отображение их содержимого
- роутинг (механизм перехода между разделами, в зависимости от адреса в адресной строке).

Навигационное меню содержит ссылки на три специальные страницы "главная" (`welcome`), "список приложений" (`apps`) и "настройки" (`settings`), которые могут быть переопределены в настройках приложения.

## Конфигурация

В разделе `pages` вы можете настроить, какие страницы использовать в качестве главной страницы приложения (параметр `pages:welcome`), страницы списка разделов (параметр `pages:apps`) и настроек (параметр `pages:settings`). По умолчанию используется страница-заглушка `/static/web-ui/dummy.js`.

```js
{
    "plugins": {

        ...

        "ThinkingHome.Plugins.WebUi.WebUiPlugin": {
            "pages": {
                "welcome": "/static/web-ui/dummy.js",
                "apps": "/static/web-ui/dummy.js",
                "settings": "/static/web-ui/dummy.js"
            }
        }
    }
}
```

## API

### `[JavaScriptResource]`

С помощью атрибута `ThinkingHome.Plugins.WebUi.Attributes.JavaScriptResourceAttribute` вы можете настроить, чтобы по заданному URL на клиент возвращался заданный файл JavaScript из ресурсов. Атрибутом необходимо отметить класс вашего плагина.

В параметрах атрибута нужно указать: URL относительно корневого адреса и путь к файлу в ресурсах DLL. Также вы можете задать дополнительные параметр `Alias` - имя, по которому можно будет обращаться к файлу в [модульной системе](#Модульная-система).

#### Пример

```csharp
[JavaScriptResource("/webui/my-page.js", "MyPlugin.Resources.my-page.js")]
public class MyPlugin : PluginBase
{

}
```

### `[CssResource]`

С помощью атрибута `ThinkingHome.Plugins.WebUi.Attributes.CssResourceAttribute` вы можете настроить, чтобы по заданному URL на клиент возвращался заданный файл CSS из ресурсов. Атрибутом необходимо отметить класс вашего плагина.

```csharp
[CssResourceAttribute("/webui/my-page.css", "MyPlugin.Resources.my-page.css")]
public class MyPlugin : PluginBase
{

}
```

## Клиентская инфраструктура

### Модульная система

В основе инфраструктуры веб-интерфейса лежит модульная система (используется библиотека [systemjs](https://github.com/systemjs/systemjs)). Все js файлы, кроме файла `/static/web-ui/index.js` (точка входа приложения), загружаются через модульную систему. Внутри своих модулей вы можете запрашивать другие модули, используя любой синтаксис, поддерживаемый библиотекой *systemjs* (например, AMD или requirejs).

Помните, что для возможности использования вашего модуля в приложении его необходимо поместить в ресурсы DLL. Также нужно добавить плагину, к которому относится модуль, атрибут `[JavaScriptResource]`, задающий URL для вашего модуля.

### Библиотеки общего назначения

В системе доступен специальный модуль `lib`, предоставляющий доступ к библиотекам общего назначения.

```js
var lib = require('lib');

// lib.$: jQuery v3 http://jquery.com
// lib.marionette - marionette.js v3 https://marionettejs.com
// lib.backbone - backbone.js http://backbonejs.org
// lib._: underscore.js http://underscorejs.org
// lib.handlebars - шаблонизатор http://handlebarsjs.com
// lib.moment - API для работы с датами и временем https://momentjs.com
```

### Добавление разделов в веб-интерфейс

Любая страница веб-интерфейса системы – это небольшая программа на языке JavaScript. Она описывает, что именно должен видеть пользователь на экране и какие действия должны быть выполнены, когда пользователь взаимодействует с элементами интерфейса. Как и остальной код приложения, разделы загружаются через systemjs.

Чтобы модуль вашего раздела мог быть использован веб-интерфейсом, необходимо, унаследовать его (с помощью функции `extend`) от объекта `AppSection` из модуля `lib.common`.

```js
var lib = require('lib');

var Section = lib.common.AppSection.extend({
    start: function() {
        // метод start выполняется, когда пользователь преходит в ваш раздел
        lib.$('body').html('Hello world!');
    }
});

module.exports = Section;
```

### Отображение содержимого страницы

Для отображения контента страницы и обработки действий пользователя вы можете использовать средства библиотеки [marionette.js](https://marionettejs.com). В прототипе `lib.common.AppSection`, от которого наследуются все разделы, доступен метод `this.application.setContentView`, в который вы можете передать экземпляр представления ([marionette view](http://marionettejs.com/docs/master/marionette.view.html)) и оно будет добавлено на страницу в область для контента.

Вы можете использовать шаблонизаторы handlebars и underscore, доступные через модуль `lib`.

Также настроены специальные правила загрузки шаблонов. Вы можете разместить содержимое шаблона в файле со специальным расширением `.tpl` и запросить его как модуль средствами systemjs. В качестве результата вы получите строку, содержащую текст шаблона.

#### Пример

```js
var lib = require('lib');

// в переменную template получаем содержимое шаблона (string)
var template = require('webui/my-template.tpl');

// описываем прототип представления
var View = lib.marionette.View.extend({
    template: lib.handlebars.compile(template)
});

var Section = lib.common.AppSection.extend({
    start: function() {
        // создаем экземпляр представления и добавляем на страницу
        var view = new View();
        this.application.setContentView(view);
    }
});

module.exports = Section;
```

### Навигация и роутинг

Веб-интерфейс представляет собой SPA (одностраничное приложение). Оно использует единый HTML документ в качестве оболочки для динамически подгружаемого контента.

В приложении настроен роутинг (маршрутизация) - механизм, который обрабатывает часть URL после первого символа `#` (решетка) и загружает содержимое соответствующего раздела.

Если пользователь укажет в адресной строке после символа `#` путь к модулю, унаследованному от `lib.common.AppSection` (или перейдет в приложении по ссылке вида `#path/to/section.js`), указанный модуль будет автоматически загружен с сервера и у него будет вызван метод `start`.

Чтобы задать дополнительные параметры для модуля, нужно добавить в конец адреса символ `?` (знак вопроса) и указать нужный набор значений, разделяя их символом `/` (слэш). Эти значения в виде строк будут переданы как входные параметры в метод `start`.

**Например**, если перейти по адресу [http://localhost:8080#static/web-ui/dummy.js?value1/2/3](http://localhost:8080#static/web-ui/dummy.js?value1/2/3) (при условии, что корневой адрес приложения - localhost:8080), с сервера будет загружен файл `static/web-ui/dummy.js`, будет создан экземпляр модуля, который в нем содержится, и у него будет вызван метод `start` с параметрами `"value1"`, `"2"`, `"3"`.

Если необходимо из одного раздела перейти в другой, используйте метод `this.application.navigate`, передав первым параметром путь к файлу, а вторым и последующими параметрами - аргументы для метода `start`. При этом автоматически изменится адрес в адресной строке браузера.

```js
var Section = lib.common.AppSection.extend({
    start: function() {
        var view = new View();

        this.listenTo(view, 'click', function() {
            // если произошло событие 'click', переходим в другой раздел
            this.application.navigate('static/web-ui/dummy.js', 'value1', 2, 3);
        });

        this.application.setContentView(view);
    }
});
```

### Клиент-серверная шина сообщений

Клиент-серверная шина сообщений - инструмент для передачи данных между сервером и клиентом (например, браузером), причем инициировать отправку сообщения может как клиент, так и сервер.

В прототипе `lib.common.AppSection`, от которого наследуются все разделы, доступен специальный объект `this.application.radio`, который предоставляет API для отправки и получения сообщений.

```js
var Section = lib.common.AppSection.extend({
    start: function() {

        // пример подписки на сообщения в канале 'test-channel'
        this.listenTo(this.application.radio, 'test-channel', function(msg) {
            console.log(msg);
        });

        // пример отправки сообщения
        this.application.radio.sendMessage('test-channel', new Date());
    }
});
```

Отправляемые сообщения будут получены подписанными обработчиками на сервере и на всех клиентах, подключенных в текущий момент.
