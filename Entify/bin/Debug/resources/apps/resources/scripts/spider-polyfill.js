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
		
		a.innerHTML = "&nbsp;" + link.innerHTML + "&nbsp;";
	} else {
		a.appendChild(clone);
	}
	var linkParentNode = link.parentNode;
	var index = whichChild(link);
	a.setAttribute('href', link.getAttribute('uri'));
	link.parentNode.removeChild(link);

	console.log(link);
	console.log(link.tagName);
	console.log(clone.innerHTML);
	
	linkParentNode.insertBefore(a, linkParentNode.childNodes[index]);

}

var btns = document.querySelectorAll('[onClick]');
for(var i = 0; i < btns.length; i++) {
	var btn = btns[i];
	btn.setAttribute('onclick', "Spidercore.execute('" + btn.getAttribute('onClick') + "')");

}