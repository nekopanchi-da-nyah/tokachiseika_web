
(function(){

   var calcTotal;
   var output;
   var inputs;
   var culcOut;
   var price;
   var round;
   var roundCD;
   var formatter = new Intl.NumberFormat();
   /*
   var formatter = new Intl.NumberFormat('ja', {
      style: 'decimal',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
   });
   /*/
   var holder = [];
   var form;
   var eTotal;
   var eHeight;
   var button;
   
   function init(){

      calcTotal = document.getElementById('total');
      output = document.getElementsByClassName('output')[0];
      inputs = output.getElementsByTagName('input');
      culcOut = output.getElementsByClassName('calc_out');
      price = output.getElementsByClassName('item_price');
      round = document.getElementsByClassName('round')[0];
      roundCD = round.dataset.round;
      form = document.getElementById('confirm');
      eTotal = document.getElementsByClassName('total')[0];
      eDiv = eTotal.getElementsByTagName('div')[0];
      eHeight = eTotal.offsetTop;
      button = document.getElementById('confirm_btn');
      
      form.addEventListener('keypress', function(e){
         const key = e.keyCode || e.charCode || 0;
         if(key == 13){
            e.preventDefault();
         }
      }, false);
      
      for(var i = 0; i < inputs.length; i++){
         holder[i] = 0;
         if(inputs[i].value != null || inputs[i].value != undefined){
            var pri = getPrice(i);
            var uni = Number(inputs[i].value);
            holder[i] = pri * uni;
            calcTotal.innerText = formatter.format(sumTotal());
         }
         (function(i){
            inputs[i].addEventListener('input', function(){
               var pri = getPrice(i);
               var uni = Number(inputs[i].value);
               var subTotal = pri * uni;
               holder[i] = sumRound(subTotal);
               
               culcOut[i].innerHTML = formatter.format(holder[i]);
               calcTotal.innerText = formatter.format(sumTotal());
            }, false);
         })(i);
      }
      
      window.addEventListener('scroll', totalFixed, false);
      
   }
   
   function getPrice(int){
      return parseFloat(price[int].dataset.value);
   }
   
   function sumRound(arg){
      
      switch(roundCD){
         case "0": 
            arg = Math.floor(arg);
            break;
         
         case "1":
            arg = Math.round(arg);
            break;
         
         case "2":
            arg = Math.ceil(arg);
            break;
      }
      return arg;
   }
   
   function sumTotal(){
      var total = 0;
      for(var i = 0; i < culcOut.length; i++){
         total += holder[i];
      }
      
      /*
      switch(roundCD){
         case "0": 
            total = Math.floor(total);
            break;
         
         case "1":
            total = Math.round(total);
            break;
         
         case "2":
            total = Math.ceil(total);
            break;
      }
      */
      
      if(total >= 1)
      {
         button.removeAttribute('disabled');
      }else{
         button.setAttribute('disabled', 'disabled');
      }
      
      return total;
   }
   
   function totalFixed(){
      if(window.scrollY > eHeight){
         eDiv.classList.add('fixed');
      }else{
         eDiv.classList.remove('fixed');
      }
   }
   
   window.addEventListener('DOMContentLoaded', init, false);
   
})();
