
(function(){

      var calcTotal;
      var output;
      var inputs;
      var culcOut;
      var price;
      var round;
      var roundCD
      var formatter = new Intl.NumberFormat();
      var holder = [];;

   function init(){

      calcTotal = document.getElementById('total');
      output = document.getElementsByClassName('output')[0];
      inputs = output.getElementsByTagName('input');
      culcOut = output.getElementsByClassName('calc_out');
      price = output.getElementsByClassName('item_price');
      round = document.getElementsByClassName('round')[0];
      roundCD = round.dataset.round;
   
      for(var i = 0; i < inputs.length; i++){
         holder[i] = 0;
         (function(i){
            inputs[i].addEventListener('input', function(){
               var pri = getPrice(i);
               var uni = Number(inputs[i].value);
               var subTotal = 0;
               
               switch(roundCD){
                  case "0": 
                     subTotal = Math.floor(pri * uni);
                     break;
                  
                  case "1":
                     subTotal = Math.round(pri * uni);
                     break;
                  
                  case "2":
                     subTotal = Math.ceil(pri * uni, 2);
                     break;
               }
               
               holder[i] = subTotal;
               culcOut[i].innerHTML = formatter.format(subTotal);
               calcTotal.innerText = formatter.format(sumTotal());
            }, false);
         })(i);
      }
   }
   
   function getPrice(int){
      return parseFloat(price[int].dataset.value);
   }
   
   function sumTotal(){
      var total = 0;
      for(var i = 0; i < culcOut.length; i++){
         total += holder[i];
      }
      return total;
   }
   
   window.addEventListener('DOMContentLoaded', init, false);
})();