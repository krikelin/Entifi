var tabBar = new TabBar({
	tabs:[
		{
			title:"Overview",
			id:"overview"
		}
	]
});
console.log(tabBar.node);
var table = document.getElementsByTagName('section')[0];
table.parentNode.insertBefore(tabBar.node, table);