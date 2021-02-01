<%@ Page language="C#" masterpagefile="page.master" codefile="program/order.cs" inherits="Order.Program" %>
<%@ MasterType VirtualPath="page.master" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="order" class="inner">
               <h2>商品注文画面</h2>
               <form runat="server">
                  <p><%= Session["お客様名称"] %></p>
                  <div class="row">
                     <div>
                        <div class="userinfo">
                           <ul>
                              <li>
                                 <label>発注元</label>
                                 <select name="branch">
                                    <% if(combo != null ){ %>
                                    <option value="<%= combo["得意先CD"] %>"><%= combo["得意先名称"] %></option>
                                    <% } %>
                                 </select>
                              </li>
                              <li>
                                 <label>納品先</label>
                                 <select name="store">
                                    <% if(combo != null ){ %>
                                    <option value="<%= combo["店舗CD"] %>"><%= combo["店舗名称"] %></option>
                                    <% } %>
                                 </select>
                              </li>
                           </ul>
                        </div>
                        
                        <div class="sort">
                           <ul>
                              <li>
                                 <label>表示順</label>
                                 <select name="order">
                                    <option value="得意先商品CD">商品コード</option>
                                 </select>
                              </li>
                           </ul>
                        </div>
                     </div>
                     
                     <div class="search">
                        <div class="input_block">
                           <p>検索条件</p>
                           <ul>
                              <li>
                                 <label>大分類</label>
                                 <select name="cate_big">
                                    <option></option>
                                 </select>
                              </li>
                              <li>
                                 <label>中分類</label>
                                 <select name="cate_mid">
                                    <option></option>
                                 </select>
                              </li>
                              <li>
                                 <label>条件1</label>
                                 <select name="">
                                    <option></option>
                                 </select>
                              </li>
                           </ul>
                        </div>
                        <div class="btn">
                           <asp:button runat="server" text="検索" onclick="Search_Click"></asp:button>
                        </div>
                     </div>
                  </div>
                  
                  
                  
               </form>
               
               <% if(itemList != null){ %>
               <div class="output">
                  <form method="POST" action="confirm.aspx">
                  <div>
                     <ul>
                        <% if( itemList.Rows.Count > 0 ){ %>
                        <% for( var i = 0; i < itemList.Rows.Count; i++ ){ %>
                        <li>
                           <div class="left">
                              <div class="upper">
                                 <p class="item_name"><%= (string)itemList.Rows[i]["得意先商品名"] %></p>
                              </div>
                              <div class="lower">
                                 <p class="item_unit"></p>
                                 <p class="unit_guid"></p>
                                 <p class="item_new"><%= (string)itemList.Rows[i]["新商品"] %></p>
                              </div>
                           </div>
                           
                           <div class="middle">
                              <div class="image">
                                 <figure>
                                    <img src="<%= Master.uploadPath + (string)itemList.Rows[i]["画像名"] %>">
                                 </figure>
                              </div>
                              <div class="upper">
                                 <p class="item_code">(<%= (string)itemList.Rows[i]["得意先商品CD"] %>)</p>
                                 <p class="standard">規格: <%= (string)itemList.Rows[i]["商品規格1"] %></p>
                              </div>
                              <div class="lower">
                                 <% if( delivDate.Rows.Count > 0 ){ %>
                                 <dl>
                                    <dt>注文日</dt>
                                    <dd><%= String.Format( "{0:yyyy/MM/dd}", delivDate.Rows[0]["発注日"] ) %></dd>
                                    <dt>納品日</dt>
                                    <dd><%= String.Format( "{0:yyyy/MM/dd}", delivDate.Rows[0]["納品日"] ) %></dd>
                                 </dl>
                                 <% } %>
                                 <p class="item_price" data-value="<%= String.Format("{0:#.#0}", itemList.Rows[i]["下代単価"]) %>">＠<%= String.Format("{0:#,###.#0}", itemList.Rows[i]["下代単価"]) %>(税抜)</p>
                              </div>
                           </div>
                           <div>
                              <input type="number" min="0" name="<%= (string)itemList.Rows[i]["自社商品CD"] %>">
                              <div class="culc">
                                 <p>金額</p>
                                 <p><span></span></p>
                              </div>
                           </div>
                        </li>
                        <% } %>
                        <% } %>
                     </ul>
                  </div>
                  <div>
                     <button name="submit" value="confirm">確認</button>
                  </div>
               </div>
               <% } %>
            </div>
</asp:Content>