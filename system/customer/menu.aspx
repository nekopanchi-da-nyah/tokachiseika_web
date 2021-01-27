<%@ Page language="C#" masterpagefile="page.master" %>
<asp:Content ContentPlaceHolderID="main_content" runat="server">
               <div id="menu">
                  <div class="login_info">
                     <ul>
                        <li><%= Session["得意先CD"] %></li>
                     </ul>
                  </div>
                  <h2>MENU</h2>
                  <div class="menu">
                  <a class="btn" href="./order.aspx"><img src="./assets/img/order_icon.png">WEB発注</a>
                  <a class="btn" href="./history.aspx"><img src="./assets/img/history_icon.png">発注履歴</a>
                  </div>
                  <div>
                     <p class="info">デモ発注システム ver1.0</p>
                  </div>
               </div>
</asp:Content>