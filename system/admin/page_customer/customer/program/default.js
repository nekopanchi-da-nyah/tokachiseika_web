/* customer default.aspx用 js */
(function(){

   function init(){
      var outputTable = document.getElementsByClassName('output')[0];
      var tbody = outputTable.getElementsByTagName('tbody')[0];
      var tr = tbody.getElementsByTagName('tr');
      
      for(var i = 0; i < tr.length; i++){
         (function(i){
            tr[i].addEventListener('dblclick', function(){
               var a = document.createElement('a');
               a.href = "edit.aspx?code=" + tr[i].dataset.href;
               a.target = "_new";
               a.click();
            }, false);
         })(i);
      }
   }
   
   window.addEventListener('DOMContentLoaded', init, false);

})();