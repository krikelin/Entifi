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

	c.View = function () {
		this.node = document.createElement('div');

	};
	c.View.prototype = new c.Observable();
	c.View.prototype.constructor = Observable;
	c.TabBar = function (data) {
		this.addToDom = function (parent, type) {
			if(type === 'prepend') {
				parent.parentNode.insertBefore(tabBar.node, parent);
			}
		};
		this.node = document.createElement('div');
		this.node.classList.add('e-tabbar');
		var tabul = document.createElement('ul');
		this.node.appendChild(tabul);
		var self = this;
		var tabs = data.tabs;
		for(var i = 0; i < data.tabs.length; i++) {
			var tab = new c.Tab(data.tabs[i], self);
			
			tabul.appendChild(tab.node);
			

		}
	};
	c.TabBar.prototype = new c.View();
	c.TabBar.prototype.constructor =  c.View;

	c.Tab = function (data, tabbar) {
		this.tabbar = tabbar;
		this.node = document.createElement('li');
		this.node.classList.add('e-tab');
		this.node.innerText = data.title;
		this.node.setAttribute('id', 'tab-'+data.id);
		console.log(data.id);
		this.node.setAttribute('data-id', data.id);
		var self = this;
		this.node.addEventListener('mousedown', function (e) {
			//c.application.activate(self.node.getAttribute('data-id'));
			var data = {id:self.node.getAttribute('data-id')};
			self.notify('activated', data);
			self.tabbar.notify('tabchanged', data);
		});
	};

	c.Tab.prototype = new c.Observable();
	c.Tab.prototype.constructor = c.Observable;
	c.Entify = function () {
		var self = this;
		this.subscriptions = [];
		this.subscribe = function (uri) {
			var promise = new c.Promise();
			self.subscriptions[uri] = promise;
			EntifyCore.subscribe(uri);
			return promise;
		}
		this.send = function (method, uri, data) {
			var xmlHttp = new XMLHttpRequest();
			var promise = new c.Promise();
			xmlHttp.onreadystatechange = function () {
				if(xmlHttp.readyState == 4) {
					if(xmlHttp.status == 200) {
						var json = JSON.parse(xmlHttp.responseText);
						promise.defer(json);
					}
				}
			};
			xmlHttp.open("GET", uri, true);
			xmlHttp.send(data);
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

