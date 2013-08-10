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
	a.appendChild(clone);

	var linkParentNode = link.parentNode;
	var index = whichChild(link);
	a.setAttribute('href', link.getAttribute('uri'));
	link.parentNode.removeChild(link);

	console.log(link);
	console.log(link.tagName);
	console.log(clone.innerHTML);
	
	linkParentNode.insertBefore(a, linkParentNode.childNodes[index]);

}