
function init(){
   var ctm = document.getElementsByName('customer')[0];
   var ofc = document.getElementsByName('office')[0];
   var dep = document.getElementsByName('department')[0];
   var pas = document.getElementsByName('password')[0];
   var cfm = document.getElementsByName('password_confirm')[0];
   
   ctm.addEventListener('change', function(){
      resetSelect(ofc);
      resetSelect(dep);
      request('M301事業所WEB', '得意先CD', ctm.value, ofc, '事業所CD', '事業所名称');
   }, false);
   
   ofc.addEventListener('change', function(){
      resetSelect(dep)
      request('M311事業所部門WEB', '事業所CD', ofc.value, dep, '事業所部門CD', '事業所部門名称' );
   }, false);
}

function resetSelect(ele){
   while(ele.firstChild){
      ele.removeChild(ele.firstChild);
   }
   ele.setAttribute('disabled', 'disabled');
}

function request(tbl, col, whe, ele, tag1, tag2){
   var str = 'table=' + tbl + '&column=' + col + '&where=' + whe + '&tag1=' + tag1 + '&tag2=' + tag2;
   var enc = encodeURI(str);
   var req = new XMLHttpRequest();
   var ary = [];
   req.open('POST', 'program/entrycustomer_request.ashx', true);
   req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
   req.send(enc);
   req.onload = function(){
      var res = req.responseText;
      
      if(res != ""){
         
         ele.removeAttribute('disabled', 'disabled');
         var tmp = res.split('!');
         
         for(var i = 0; i < tmp.length; i++){
            ary[i] = tmp[i].split(',');
         }
         
         ele.appendChild(document.createElement('option'));
         
         for(var i = 0; i < ary.length; i++){
            var opt = document.createElement('option');
            ele.appendChild(opt);
            opt.setAttribute('value', ary[i][0] );
            opt.innerText = ary[i][0] + ary[i][1];
         }
      }
   }
}

window.addEventListener('DOMContentLoaded', init, false);