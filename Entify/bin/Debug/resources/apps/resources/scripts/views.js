(function(c) {
	c.application = {
		onready: null,
		ontabchange: function (id) {
			var sections = document.querySelectorAll('section');
			for(var i = 0; i < sections.length; i++) {
				sections[i].style.display = sections[i].getAttribute('id') === id ? 'block' : 'none';
			}
		},
		onargumentschanged: function (args) {
			Entify.subscribe(args.slice(0, 3).join(':'));
			var d = document.querySelector('#template').innerHTML;
			var template = slab.compile(d);
			for(var key in template) {
				if(template[key] instanceof Function) {
					console.log("T");
					document.getElementById('content-'+key).innerHTML = "";
					
				}
			}
		},
		onrecievedata: function (data) {

			var d = document.querySelector('#template').innerHTML;
			var template = slab.compile(d);
			for(var key in template) {
				if(template[key] instanceof Function) {
					console.log("T");
					document.getElementById('content-'+key).innerHTML = template[key]({data:data, uri:Entify.getUri().split(':')});
					
				}
			}
			console.log("Data recieved");
		},
		activate: function (id) {
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
			if(c.application.ontabchange instanceof Function)
				c.application.ontabchange.call(this, id);
					
		}
	};
	c.View = function () {
		this.node = document.createElement('div');

	};
	c.TabBar = function (data) {
		this.node = document.createElement('div');
		this.node.classList.add('e-tabbar');
		var tabul = document.createElement('ul');
		this.node.appendChild(tabul);

		var tabs = data.tabs;
		for(var i = 0; i < data.tabs.length; i++) {
			var tab = new c.Tab(data.tabs[i]);
			tabul.appendChild(tab.node);
			

		}
		c.application.activate('overview');
	};
	c.TabBar.prototype = new c.View();
	c.TabBar.prototype.constructor =  c.View;

	c.Tab = function (data) {
		this.node = document.createElement('li');
		this.node.classList.add('e-tab');
		this.node.innerText = data.title;
		this.node.setAttribute('id', 'tab-'+data.id);
		console.log(data.id);
		this.node.setAttribute('data-id', data.id);
		var self = this;
		this.node.addEventListener('mousedown', function (e) {
				c.application.activate(self.node.getAttribute('data-id'));
				
			});
	};
})(this);
Entify.subscribe(Entify.getUri());
application.activate('overview');