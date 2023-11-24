const baseURL = "https://localhost:44349/ECommerce/Data";
const newProduct = document.getElementById("newProducts2");
const btnContainer = document.getElementById("btnContainer");
let products = [];
let index = 0;
let pages = [];

const displayProducts = (products) => {
    const htmlTemplate = products
        .map((product) => {
            return `
            <div class="col-md-4">
                <div class="card">
                       <img class="card-img-top" src="${product.image}" alt="card image">
                       <div class="card-body">
                               <h5 class="card-title">${product.name}</h5>
                               <a href="" class="btn btn-primary">Go to Product</a>
                       </div>
                </div>
            </div>`;
        })
        .join("");
    newProduct.innerHTML = htmlTemplate;
};
//const loadProducts = $.ajax({
//    url: '@Url.Action("Data", "ECommerce", "ECommerce")',
//    type: "GET",
//    dataType: "json",
//    cache: false,
//    success: function (results) {
//        products = results;
//        displayProducts(products);
//    },
//    error: function () {
//        alert("Error occured");
//    },
//});
const loadProducts = async () => {
    try {
        const response = await fetch(baseURL);
        products = await response.json();
        displayProducts(products);
        //console.log(products);
        if (!response.ok)
            throw new Error(`${products.message} ${response.status}`);
        return products;
    } catch (err) {
        console.log(err);
    }
};

const paginate = (products) => {
    const pageItems = 9;
    const pagesCount = Math.ceil(products.length / pageItems);
    const newProducts = Array.from({ length: pagesCount }, (_, pageindex) => {
        const start = pageindex * pageItems;
        return products.slice(start, start + pageItems);
    });
    return newProducts;
};
const displayButtons = (btnContainer, pages, activeIndex) => {
    let btns = pages.map((_, pageIndex) => {
        return `<button class="pageBtn 
        ${activeIndex === pageIndex ? "activeBtn" : "null"}
        " data-index= "${pageIndex}">
        ${pageIndex + 1}</button>`;
    });
    btns.push(`<button class="prevBtn">
    <i class="fa fa-arrow-left" aria-hidden="true"></i></button>`);
    btnContainer.innerHTML = btns.join("");
};

btnContainer.addEventListener("click", function (e) {
    if (e.target.classList.contains("btnContainer")) return;
    if (e.target.classList.contains("pageBtn")) {
        index = parseInt(e.target.dataset.index);
    }
    if (e.target.classList.contains("nextBtn")) {
        index++;
        if (index > pages.length - 1) {
            index = 0;
        }
    }
    if (e.target.classList.contains("prevBtn")) {
        index--;
        if (index < 0) {
            index = pages.length - 1;
        }
    }
    displayProducts(pages[index]);
    displayButtons(btnContainer, pages, index);
});
//document.querySelector("#menu-toggle").addEventListener("click", function (e) {
//    e.preventDefault();
//    const wrapper = document.querySelector("#wrapper");
//    wrapper.classList.toggle("toggled");
//});
window.addEventListener("load", async () => {
    const products = await loadProducts();
    pages = paginate(products);
    displayProducts(pages[index]);
    displayButtons(btnContainer, pages, index);
});
$(document).ready(function () {
    $("#myTable").DataTable({
        language: {
            url: "//cdn.datatables.net/plug-ins/1.13.7/i18n/ar.json",
        },
    });
    $("#userData").DataTable({
        ajax: {
            url: "/Admin/User/GetAll",
        },
        columns: [
            { data: "name", width: "25%" },
            { data: "email", width: "25%" },
            { data: "phoneNumber", width: "25%" },
            { data: "company.name", width: "25%" },
            { data: "role", width: "25%" },
        ],
    });
});
