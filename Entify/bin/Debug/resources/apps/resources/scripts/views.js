(function(c) {
	

	c.View = function () {
		this.node = document.createElement('div');

	};
	c.View.prototype = new c.Observable();
	c.View.prototype.constructor = Observable;
	c.BaseButton = function (uri, args) {
		this.node = document.createElement('button');
		var self = this;
		this.node.classList.add('btn');
		var self = this;
		self.node.addEventListener('click', function (e) {
			self.notify('click', e);
		})
		console.log(args);
		this.applyOptions = function (args) {
			if(typeof(args) !== 'undefined') {
				if(typeof(args.text) !== 'undefined') {
					self.node.innerHTML = "";
					var span = document.createElement('span');
					var textNode = document.createTextNode(args.text);
					span.appendChild(textNode);
					if(typeof(args.icon) !== 'undefined') {
						var icon = document.createElement('i');
						self.node.appendChild(icon);
						icon.classList.add('icon');
						icon.classList.add('icon-' + args.icon);
					}
					self.node.appendChild(span);
				}
			}
		}	
	};
	c.BaseButton.forNode = function (uri, args) {
		return new c.BaseButton(uri, args);
	};
	c.BaseButton.prototype = new c.View();
	c.BaseButton.prototype.constructor = c.View;
	
	c.FollowButton = function (node, args) {
		var self = this;
		this.enode = node;
		console.log(args);
		this.node.classList.add('btn-follow');
		
		this.addEventListener('click', function () {
			c.application.entify.send('FOLLOW', self.enode.uri, {}).done(function (node) {
				console.log(e);
				if(e.data.following) {
					self.node.classList.add('btn-subscribed');
					self.applyOptions({text:'Following'});
				} else {
					self.node.classList.remove('btn-subscribed');
					self.applyOptions({text:'Follow'});

				}
				console.log('subscribed');
			});
		})
		this.applyOptions(args);
		if(this.enode.data.following) {
			this.node.classList.add('btn-subscribed');
			this.applyOptions({text: 'Following'})
		}
	};
	c.FollowButton.prototype = new c.BaseButton();
	c.FollowButton.prototype.constructor = c.BaseButton;
	
	c.FollowButton.forNode = function (node, args) {

		
		
		args.text = 'Follow';
		return new c.FollowButton(node, args);
	};
	


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

