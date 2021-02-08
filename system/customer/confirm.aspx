<%@ Page language="C#" masterpagefile="page.master" codefile="program/confirm.cs" inherits="ConfirmPage.Program" %>
<%@ MasterType VirtualPath="page.master" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="order" class="inner">
               <h2>注文確認画面</h2>
               <div class="output">
                  <% if(strPath == ""){ %>
                  <p style="text-align: center; font-size: 1.1rem; color: #f00; ">
                     <strong>以下の内容で注文いたしました。<br>注文内容ご確認ください。</strong>
                  </p>
                  <ul>
                     <% for(var i = 0; i < orders.Count; i++){ %>
                     <li>
                        <div class="image">
                           <figure>
                              <img src="<%= orders[i].strImageName %>">
                           </figure>
                        </div>
                        <div class="box">
                           <div class="upper">
                              <p class="item_name"><%= orders[i].strItemName  %></p>
                           </div>
                           <div class="flex">
                              <div class="left">
                                 <div class="lower">
                                    <p class="item_unit"></p>
                                    <p class="unit_guid"></p>
                                 </div>
                              </div>
                              
                              <div class="right">
                                 <div class="upper">
                                    <p class="item_code">(<%= orders[i].strClientItemCD  %>)</p>
                                 </div>
                                 <div class="lower">
                                    <dl>
                                       <dt>注文日</dt>
                                       <dd><%= String.Format( "{0:yyyy/MM/dd}", orders[i].dateOrderDate ) %></dd>
                                       <dt>納品日</dt>
                                       <dd><%= String.Format( "{0:yyyy/MM/dd}", orders[i].dateDeliveryDate ) %></dd>
                                    </dl>
                                    <p class="item_price" data-value="<%= String.Format("{0:#.#0}", orders[i].deciOrderSaleUnit) %>">＠<%= String.Format("{0:#.#0}", orders[i].deciOrderSaleUnit) %>(税抜)</p>
                                    <div class="inputs">
                                       <label>数量</label>
                                       <p style="width: 2rem; text-align: right; font-weight: bold; margin-right: 1rem;"><%= orders[i].intOrderUnit %></p>
                                       <div class="culc">
                                          <p>金額</p>
                                          <p style="width: 4rem; text-align: right; font-weight: bold;"><span class="calc_out"><%= String.Format("{0:#.#0}", orders[i].deciOrderSales) %></span></p>
                                       </div>
                                    </div>
                                 </div>
                              </div>
                           </div>
                        </div>
                     </li>
                     <% } %>
                  </ul>
                  <% } else { %>
                  <p style="text-align: center; font-size: 1.1rem; color: #f00; ">
                     <strong>以下の内容で帳票を作成致しました。</strong>
                  </p>
                  <div>
                     <iframe class="pdf" src="<%= strPath %>"></iframe>
                  </div>
                  <% } %>
                  <% if(strPath == ""){ %>
                  <form runat="server">
                     <div style="text-align: center">
                        <asp:button runat="server" text="帳票印刷" onclick="Print_Click" class="createpdf"></asp:button>
                     </div>
                  </form>
                  <% } %>
               </div>
               <div class="total">
                  <div>
                     <p>注文合計</p>
                     <p id="total"><%= String.Format("{0: #,###}", total) %></p>
                  </div>
               </div>
               <div class="btns">
                  <a href="menu.aspx"><button class="home">HOMEに戻る</button></a>
                  <a href="order.aspx"><button class="order">発注画面に戻る</button></a>
               </div>
            </div>
</asp:Content>