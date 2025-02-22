<%@ Page language="C#" masterpagefile="page.master" codefile="program/order.cs" inherits="OrderPage.Program" %>
<%@ MasterType VirtualPath="page.master" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
            <script src="program/order.js"></script>
            <script>
            function check(){
               if(window.confirm('注文を確定します。よろしいですか？')){ // 確認ダイアログを表示
                  return true; // 「OK」時は送信を実行
               }else{ // 「キャンセル」時の処理
                  window.alert('キャンセルされました'); // 警告ダイアログを表示
                  return false; // 送信を中止
               }
            }
            </script>
            <div id="order" class="inner">
               <h2>商品注文画面</h2>
               <form runat="server">
                  <div class="row">
                     <p><%= Session["お客様名称"] %></p>
                     <% if(closeDay.ToString() != "0001/01/01 0:00:00"){ %>
                     <p>発注締日時: <span><%= closeDay %></span></p>
                     <% } %>
                  </div>
                  <div class="row">
                     <div>
                        <div class="userinfo">
                           <ul>
                              <li>
                                 <label class="round" data-round="<%= combo["端数処理区分CD"] %>">発注元</label>
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
                                 <select name="orderby">
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
                                 <label>販売停止商品</label>
                                 <select name="item_condition">
                                    <option value="true">表示しない</option>
                                    <option value="false">表示する</option>
                                 </select>
                              </li>
                           </ul>
                        </div>
                        <div class="btn">
                           <asp:button runat="server" text="検索" onclick="Search_Click"></asp:button>
                        </div>
                     </div>
                  </div>
                  
                  <div class="total">
                     <div>
                        <p>注文合計</p>
                        <p id="total"></p>
                     </div>
                  </div>
                  
               </form>
               
               <% if(itemList != null){ %>
               <div class="output">
                  <form method="POST" action="confirm.aspx" id="confirm" onSubmit="return check()">
                     <div>
                        <p>対象件数は <span><%= itemList.Rows.Count %></span>件です</p>
                        <ul>
                           <% if( itemList.Rows.Count > 0 ){ %>
                           <% for( var i = 0; i < itemList.Rows.Count; i++ ){ %>
                           <% if((string)itemList.Rows[i]["販売停止FLG"] == "1"){ %>
                           <li class="closeitem">
                           <% }else{ %>
                           <li>
                           <% } %>
                              <div class="image">
                                 <figure>
                                    <% if( itemList.Rows[i]["画像名"] != DBNull.Value){ %>
                                    <img src="../../upload/img/<%= itemList.Rows[i]["画像名"] %>">
                                    <% }else{ %>
                                    <img src="../../upload/img/noimage.png">
                                    <% } %>
                                 </figure>
                              </div>
                              <div class="box">
                                 <div class="upper">
                                    <p class="item_name">
                                       <% if(itemList.Rows[i]["新商品"] != ""){ %>
                                       <span class="newitem"><%= itemList.Rows[i]["新商品"] %></span>
                                       <% }else if((string)itemList.Rows[i]["販売停止FLG"] == "1"){ %>
                                       <span class="closeitem">販売停止商品</span>
                                       <% } %>
                                       <%= (string)itemList.Rows[i]["得意先商品名"] %>
                                    </p>
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
                                          <p class="item_code">(<%= itemList.Rows[i]["得意先商品CD"] %>)</p>
                                          <p class="standard">規格: <%= itemList.Rows[i]["商品規格2"] %></p>
                                       </div>
                                       <div class="lower">
                                          <dl>
                                             <dt>注文日</dt>
                                             <dd><%= String.Format( "{0:yyyy/MM/dd}", orderDay ) %></dd>
                                             <dt>納品日</dt>
                                             <dd><%= String.Format( "{0:yyyy/MM/dd}", delivDay ) %></dd>
                                          </dl>
                                          <p class="item_price" data-value="<%= String.Format("{0:#.#0}", itemList.Rows[i]["売上単価"]) %>">＠<%= String.Format("{0:#,###.#0}", itemList.Rows[i]["売上単価"]) %>(税抜)</p>
                                          <div class="inputs">
                                             <label>数量</label>
                                             <% if((string)itemList.Rows[i]["販売停止FLG"] == "1"){ %>
                                             <input type="number" min="0" name="<%= (string)itemList.Rows[i]["得意先商品CD"] %>" tabindex="<%= i + 1 %>" value="<%= itemList.Rows[i]["数量"] %>" disabled >
                                             <% }else{ %>
                                             <input type="number" min="0" name="<%= (string)itemList.Rows[i]["得意先商品CD"] %>" tabindex="<%= i + 1 %>" value="<%= itemList.Rows[i]["数量"] %>" >
                                             <% } %>
                                             <div class="culc">
                                                <p>金額</p>
                                                <p style="width: 4rem; text-align: right; font-weight: bold;"><span class="calc_out"><%= String.Format("{0: #,###.00}", itemList.Rows[i]["売上金額"]) %></span></p>
                                             </div>
                                          </div>
                                       </div>
                                    </div>
                                 </div>
                              </div>
                           </li>
                           <% } %>
                           <% } %>
                        </ul>
                     </div>
                     <div class="btns">
                        <a href="menu.aspx"><button type="button" value="cancel" class="end">戻る</button></a>
                        <button name="submit" value="confirm" id="confirm_btn" disabled class="puls">発注</button>
                     </div>
                  </form>
               </div>
               <% } %>
            </div>
</asp:Content>