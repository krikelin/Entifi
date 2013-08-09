(function(c) {
	
	
	c.View = function () {
		this.node = document.createElement('div');

	};
	c.View.prototype = new c.Observable();
	c.View.prototype.constructor = Observable;
	c.TabBar = function (data) {
		this.addToDom = function (parent, type) {
			if(type === 'prepend') {
				parent.parentNode.insertBefore(self.node, parent);
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
	
})(this);

