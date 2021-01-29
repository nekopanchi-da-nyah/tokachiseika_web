<%@ Page language="C#" masterpagefile="page.master" codefile="program/order.cs" inherits="Order.Program" %>
<%@ MasterType VirtualPath="page.master" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="order" class="inner">
               <h2>商品注文画面</h2>
               <div>
                  <div class="userinfo">
                     <p><%= Session["お客様名称"] %></p>
                     <ul>
                        <li>
                           <label></label>
                           <select disabled>
                              <option></option>
                           </select>
                        </li>
                     </ul>
                  </div>
               </div>
               <div class="output">
               </div>
            </div>
</asp:Content>