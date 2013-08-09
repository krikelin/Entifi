(function(c) {
	c.Observable = function () {
		var self = this;
		this._observers = [];
		this.addEventListener = function (evt, callback) {
			var event = new c.Observer(evt, callback);
			self._observers.push(event);
		};
		this.notify = function(evt, args) {
			for(var i = 0; i < self._observers.length; i++) {
				var observer = self._observers[i];
				console.log(evt, args);
				if(observer.name === evt)
					observer.callback.call(self, args);
			}
		};
	};
	c.Observer = function (name, callback) {
		this.name = name;
		this.callback = callback;
	}
	c.Promise = function () {
		var self = this;
		this.defer = function (a) {
			console.log("defer");
			if(a.status === 'OK') {
				if(self._success instanceof Function)
				self._success(a);
			} else {
				self._fail(a);
			}
			self._done(a);
			return self;
		};
		this.done = function (callback) {
			self._done = callback;
		};
		this._done = function () {};	
		this._fail = function () {};	
		this._all = function () {};	
	};
	c.Application = function () {
		this.entify = new c.Entify();
		this.onready = null;
		var self = this;
		this.addEventListener('reply', function (e) {
			console.log(e);
			try {
				self.entify.conversations[e.uri].defer(e);
			} catch (e) {
				//alert(e);
			}
		});
		this.addEventListener('tabchange', function (id) {
			var sections = document.querySelectorAll('section');
			for(var i = 0; i < sections.length; i++) {
				sections[i].style.display = sections[i].getAttribute('id') === id ? 'block' : 'none';
			}
		});
		this.addEventListener('argumentschanged', function (args) {
			Entify.subscribe(args.slice(0, 3).join(':'));
			var d = document.querySelector('#template').innerHTML;
			var template = slab.compile(d);
			for(var key in template) {
				if(template[key] instanceof Function) {
					console.log("T");
					document.getElementById('content-'+key).innerHTML = "";
					
				}
			}
		});
		this.activate = function (id) {
			var tabs = document.querySelectorAll('.e-tab');
			for(var i = 0; i < tabs.length; i++) {
				var tab = tabs[i];
				var tid = tab.getAttribute('data-id');
				if(tid === id) {
					tab.classList.add('e-tab-active');
				} else {
					tab.classList.remove('e-tab-active');
				}
			}
			
			c.application.notify('tabchange', id);
		
			var sections = document.querySelectorAll('section');
			for(var i = 0; i < sections.length; i++) {
				sections[i].style.display = sections[i].getAttribute('id') === id ? 'block' : 'none';
			}
			
			
		};
		this.nodes = [];
		this.addEventListener('recievedata', function (data) {
			console.log(data);
			data.status = 'OK';
			if(typeof(data) === 'undefined') return;
			var node = new c.Node(data.uri, data.data);
			self.nodes.push(node);
			console.log(c.entify.subscriptions);
			self.entify.subscriptions[data.uri].defer(data);
			/**/
			console.log("Data recieved");
		});
	};
	c.Application.prototype = new c.Observable();
	c.Application.prototype.constructor = c.Observable;
	c.Entify = function () {
		var self = this;
		this.subscriptions = [];
		this.conversations = [];
		this.subscribe = function (uri) {
			var promise = new c.Promise();
			self.subscriptions[uri] = promise;
			EntifyCore.subscribe(uri);
			return promise;
		}
		this.send = function (method, uri, data) {
			var xmlHttp = new XMLHttpRequest();

			var promise = new c.Promise();
			self.conversations[uri] = promise;
			EntifyCore.send(method, uri, JSON.stringify(data));
			return promise;
		};
	};
	c.Entify.prototype = new c.Observable();
	c.Entify.prototype.constructor = c.Observable;
	c.Node = function (uri, data) {
		this.uri = uri;
		this.data = data;
	};
	c.Node.prototype = new c.Observable();
	c.Node.prototype.constructor = c.Observable;
	c.application = new c.Application();
	c.entify = new c.Entify();
})(this);