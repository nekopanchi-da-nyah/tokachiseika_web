<%@ Page language="C#" masterpagefile="page.master" codefile="program/confirm.cs" inherits="Confirm.Program" %>
<%@ MasterType VirtualPath="page.master" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="order" class="inner">
               <h2>注文確認画面</h2>
               <div class="output">
                  <p style="text-align: center; font-size: 1.1rem; color: #f00; ">
                     <strong>以下の内容で注文いたします。<br>注文内容ご確認ください。</strong>
                  </p>
                  <ul>
                     <% foreach(var o in orderList){ %>
                     <%--
                     <li>
                        <div class="image">
                           <figure>
                              <img src="<%= o.strImageName %>">
                           </figure>
                        </div>
                        <div class="box">
                           <div class="upper">
                              <p class="item_name"><%= o.strItemName  %></p>
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
                                    <p class="item_code">(<%= o.strClientItemCD  %>)</p>
                                 </div>
                                 <div class="lower">
                                    <dl>
                                       <dt>注文日</dt>
                                       <dd><%= String.Format( "{0:yyyy/MM/dd}", orderList[i].dateOrderDate ) %></dd>
                                       <dt>納品日</dt>
                                       <dd><%= String.Format( "{0:yyyy/MM/dd}", orderList[i].dateDelivDate ) %></dd>
                                    </dl>
                                    <p class="item_price" data-value="<%= String.Format("{0:#.#0}", orderList[i].decSalesUnit) %>">＠<%= String.Format("{0:#.#0}", orderList[i].decSalesUnit) %>(税抜)</p>
                                    <div class="inputs">
                                       <label>数量</label>
                                       <p style="width: 2rem; text-align: right; font-weight: bold; margin-right: 1rem;"><%= orderList[i].intUnit %></p>
                                       <div class="culc">
                                          <p>金額</p>
                                          <p style="width: 4rem; text-align: right; font-weight: bold;"><span class="calc_out"><%= String.Format("{0:#.#0}", orderList[i].decSales) %></span></p>
                                       </div>
                                    </div>
                                 </div>
                              </div>
                           </div>
                        </div>
                     </li>
                     --%>
                     <% } %>
                  </ul>
               </div>
               <form runat="server">
                  <div class="total">
                     <div>
                        <p>注文合計</p>
                        <p id="total"><%= String.Format("{0: #,###}", total) %></p>
                     </div>
                  </div>
                  <div class="btns">
                     <a href="javascript:history.back()">戻る</a>
                     <asp:button runat="server" text="発注確定"></asp:button>
                  </div>
               </form>
            </div>
</asp:Content>