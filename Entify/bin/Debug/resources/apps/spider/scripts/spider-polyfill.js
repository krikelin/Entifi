function whichChild(elem){
    var  i= 0;
    while((elem=elem.previousSibling)!=null) ++i;
    return i;
}
var links = document.querySelectorAll('[uri]');
for(var i =0 ; i < links.length; i++) {
	var link = links[i];
	var clone = link.cloneNode(true);

	var a = document.createElement('a');

	if(link.tagName.toLowerCase() === 'alink') {
		
		a.innerHTML = link.innerHTML;
	} else {
		a.appendChild(clone);
	}
	console.log(clone.style);
	
	var linkParentNode = link.parentNode;
	var index = whichChild(link);
	a.setAttribute('href', link.getAttribute('uri'));
	link.parentNode.removeChild(link);

	console.log(link);
	console.log(link.tagName);
	console.log(clone.innerHTML);
	a.style.display = 'block';
	linkParentNode.insertBefore(a, linkParentNode.childNodes[index]);

}

var btns = document.querySelectorAll('[onClick]');
for(var i = 0; i < btns.length; i++) {
	var btn = btns[i];
	btn.setAttribute('onclick', "Spidercore.execute('" + btn.getAttribute('onClick') + "')");

}

var sections = document.querySelectorAll('section');
var options = {tabs: []};
for(var i = 0; i < sections.length; i++) {
	var section = sections[i];
	var tab = {
		id: section.getAttribute('id'),
		title: section.getAttribute("title")
	};
	options.tabs.push(tab);
}
var tabBar = new TabBar(options);
tabBar.addToDom(document.body, options);
console.log(tabBar);
application.activate('overview');