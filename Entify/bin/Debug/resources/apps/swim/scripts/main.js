

application.entify.subscribe(EntifyCore.getUri()).done(function (args) {
	var node = args;
	console.log("A");
	var d = document.querySelector('#template').innerHTML;
	var template = slab.compile(d);
	for(var key in template) {
		if(template[key] instanceof Function) {
			console.log("T");
			console.log(args);
			document.getElementById('content-'+key).innerHTML = template[key]({data:args.data, uri:args.uri});
			
		}
	}
	console.log(args);
	var tabBar = new TabBar({
		tabs: [
			{ 
				id:'overview',
				title:'Overview'
			}
		]
	});
	tabBar.addToDom(document.body, 'prepend');
	tabBar.addEventListener('tabchanged', function(e) {
		application.activate(e.id);
	});
	application.activate('overview');

	var followButton = FollowButton.forNode(node, {});
	document.querySelector('#toolbar').appendChild(followButton.node);
})