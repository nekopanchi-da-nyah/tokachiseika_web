<%@ Page language="C#" masterpagefile="page.master" codefile="program/dashboard.cs" inherits="Dashboard.Program" %>
<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="dashboard" class="inner">
               <h2>menu</h2>
               <div class="output">
                  
                  <div class="box">
                     <h3>受注管理</h3>
                     <hr>
                     <div class="wrap">
                        <ul>
                        
                           <li>
                              <a href="">
                                 <figure>
                                    <img src="<%= imgPath %>order_icon_2.png">
                                    <figcaption>新規受注</figcaption>
                                 </figure>
                              </a>
                           </li>
                           
                           <li>
                              <a href="">
                                 <figure>
                                    <img src="<%= imgPath %>history_icon.png">
                                    <figcaption>受注履歴</figcaption>
                                 </figure>
                              </a>
                           </li>
                           
                           <li>
                              <a href="">
                                 <figure>
                                    <img src="<%= imgPath %>quetion_icon.png">
                                    <figcaption>未定受注</figcaption>
                                 </figure>
                              </a>
                           </li>
                           
                        </ul>
                     </div>
                  </div>
                  
                  <div class="box">
                     <h3>商品管理</h3>
                     <hr>
                     <div class="wrap">
                        <ul>
                        
                           <li>
                              <a href="">
                                 <figure>
                                    <img src="<%= imgPath %>item_icon.png">
                                    <figcaption>商品</figcaption>
                                 </figure>
                              </a>
                           </li>
                           
                           <li>
                              <a href="">
                                 <figure>
                                    <img src="<%= imgPath %>link_icon.png">
                                    <figcaption>得意先別商品</figcaption>
                                 </figure>
                              </a>
                           </li>
                           
                           <li>
                              <a href="">
                                 <figure>
                                    <img src="<%= imgPath %>cooperation_icon.png">
                                    <figcaption>基幹連携</figcaption>
                                 </figure>
                              </a>
                           </li>
                           
                        </ul>
                     </div>
                  </div>
                 
                  <div class="box">
                     <h3>得意先管理</h3>
                     <hr>
                     <div class="wrap">
                        <ul>
                        
                           <li>
                              <a href="page_customer/customer/default.aspx">
                                 <figure>
                                    <img src="<%= imgPath %>account_icon.png">
                                    <figcaption>お客様</figcaption>
                                 </figure>
                              </a>
                           </li>
                           
                           <li>
                              <a href="">
                                 <figure>
                                    <img src="<%= imgPath %>customer_icon.png">
                                    <figcaption>得意先</figcaption>
                                 </figure>
                              </a>
                           </li>
                           
                           <li>
                              <a href="">
                                 <figure>
                                    <img src="<%= imgPath %>store_icon.png">
                                    <figcaption>店舗(納品先)</figcaption>
                                 </figure>
                              </a>
                           </li>
                           
                        </ul>
                     </div>
                  </div>
                  
                  <div class="box">
                     <h3>お知らせ</h3>
                     <div>
                        <p>デモ管理です。</p>
                     </div>
                  </div>
                  
               </div>
            </div>
</asp:Content>