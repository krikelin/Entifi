

application.entify.subscribe(EntifyCore.getUri()).done(function (args) {
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
})