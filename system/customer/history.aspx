<%@ Page language="C#" masterpagefile="page.master" codefile="program/history.cs" inherits="HistoryPage.Program" %>
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
               <h2>注文履歴画面</h2>
               <form runat="server">
                  <div class="row">
                     <p><%= Session["お客様名称"] %></p>
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
                                    <option value="注文年月日">発注日</option>
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
                                 <label>条件1</label>
                                 <select name="cate_big">
                                    <option></option>
                                 </select>
                              </li>
                              <li>
                                 <label>条件2</label>
                                 <select name="cate_mid">
                                    <option></option>
                                 </select>
                              </li>
                              <li>
                                 <label>条件3</label>
                                 <select name="cate_big">
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
               
               <div class="output">
                  <form method="POST" action="confirm.aspx" id="confirm" onSubmit="return check()">
                     <div>
                        <p>対象件数は <span></span>件です</p>
                        <ul>
                        </ul>
                     </div>
                     <div class="btns">
                        <a href="menu.aspx"><button type="button" value="cancel" class="end">戻る</button></a>
                        <button name="submit" value="confirm" id="confirm_btn" disabled class="puls">発注</button>
                     </div>
                  </form>
               </div>
            </div>
</asp:Content>