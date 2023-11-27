const baseURL = "https://localhost:44349/ECommerce/Home/Data";
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
                            <div class="cardHeader">
                                <ul class="icons">
                                    <li><i class="fa fa-heart"></i></li>
                                    <li>
                                        <a href="cart.html">
                                            <i class="fa fa-shopping-bag"></i>
                                        </a>
                                    </li>
                                    <li> <i class="fa fa-search"></i></li>
                                </ul>
                                <img class="card-img-top" src="${product.img}" alt="card image">
                            </div>
                            <div class="card-body">
                                <h5>${product.name}</h5>
                                <h6>${product.type}</h6>
                                <h7 class="price">السعر: ${product.price}} رس</h7>
                                <a asp-action="Detail" class="btn btn-primary" asp-route-id="${product.id}">تفاصيل المنتج</a>
                                <div class="rating">
                                    <i class="fa fa-star-o"></i>
                                    <i class="fa fa-star-o"></i>
                                    <i class="fa fa-star-o"></i>
                                    <i class="fa fa-star-o"></i>
                                    <i class="fa fa-star-o"></i>
                                </div>
                            </div>
                        </div>
                    </div>
                    `;
        })
        .join("");
    newProduct.innerHTML = htmlTemplate;
};

const loadProducts = async () => {
    try {
        const response = await fetch(baseURL);
        products = await response.json();
        products = products.data;
        console.log(products);
        displayProducts(products);
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

window.addEventListener("load", async () => {
    const products = await loadProducts();
    pages = paginate(products);
    displayProducts(pages[index]);
    displayButtons(btnContainer, pages, index);
});
