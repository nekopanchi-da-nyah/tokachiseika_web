<%@ Page language="C#" masterpagefile="page.master" %>
<%@ MasterType VirtualPath="page.master" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="usermenu" class="inner">
               <h2>menu</h2>
               <div class="output">
                  <ul>
                     <li>
                        <a href="order.aspx">
                           <figure>
                              <img src="<%= Master.imgPath %>item_icon.png">
                              <figcaption>商品発注</figcaption>
                           </figure>
                        </a>
                     </li>
                     <li>
                        <a href="history.aspx">
                           <figure>
                              <img src="<%= Master.imgPath %>history_icon.png">
                              <figcaption>発注履歴</figcaption>
                           </figure>
                        </a>
                     </li>
                  </ul>
               </div>
            </div>
</asp:Content>