<%@ Page language="C#" masterpagefile="../page.master" codefile="program/entrycustomer.cs" inherits="EntryCustomer.Program" %>
<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="entry_customer">
               <h2>WEBユーザー新規登録</h2>
               
               <div class="output">
                  <form runat="server" method="POST">
                     <ul>
                        <li>
                           <label>ログインID</label>
                           <input name="customer_code" type="text" required min="6" max="8" />
                           <span>半角英数6～8文字</span>
                        </li>
                        <% if( idValid != "" ){ %>
                        <span style="color: red; padding-left: 0.5rem; margin-left: 0.5rem; font-size: 0.8rem;"> <%= idValid %> </span>
                        <% } %>
                        <li>
                           <label>ユーザー名</label>
                           <input name="customer_name" type="text" required min="8" max="12" />
                        </li>
                        <li>
                           <label>パスワード</label>
                           <input name="password" type="password" required min="8" max="12" />
                           <span>半角英数8～12文字</span>
                        </li>
                        <li>
                           <label>パスワード確認</label>
                           <input name="password_confirm" type="password" required />
                           <span>再度入力</span>
                        </li>
                        <% if( psValid != "" ){ %>
                        <span style="color: red; padding-left: 0.5rem; margin-left: 0.5rem; font-size: 0.8rem;"> <%= psValid %> </span>
                        <% } %>
                        
                        <li>
                           <label>得意先</label>
                           <select name="customer">
                              <option value=""></option>
                              <% for(var i = 0; i < customerDt.Rows.Count; i++ ){ %>
                              <option value="<%= customerDt.Rows[i]["得意先CD"].ToString() %>"><%= customerDt.Rows[i]["得意先CD"].ToString() + customerDt.Rows[i]["得意先名"].ToString()  %></option>
                              <% } %>
                           </select>
                        </li>
                        <li>
                           <label>事業所</label>
                           <select name="office" disabled>
                              <option></option>
                           </select>
                        </li>
                        <li>
                           <label>部門</label>
                           <select name="department" disabled>
                              <option></option>
                           </select>
                        </li>
                     </ul>
                     <div class="btn">
                        <asp:button runat="server" text="登録" onclick="Entry_Click"></asp:button>
                     </div>
                  </form>
               </div>
               
            </div>
            <script src="program/entrycustomer.js"></script>
</asp:Content>