(function () {
    const root = document.querySelector('.aliexpress-product-selector');
    if (!root) {
        return;
    }

    const productIdField = document.getElementById('aliexpressProductId');
    const productDataField = document.getElementById('aliexpressProductData');
    const selectedProductBlock = document.getElementById('aliexpressSelectedProduct');
    const noSelectionBlock = document.getElementById('aliexpressNoSelection');
    const selectedImage = document.getElementById('aliexpressSelectedImage');
    const selectedTitle = document.getElementById('aliexpressSelectedTitle');
    const selectedPrice = document.getElementById('aliexpressSelectedPrice');
    const selectedId = document.getElementById('aliexpressSelectedId');
    const searchModal = document.getElementById('aliexpressSearchModal');
    const searchKeyword = document.getElementById('aliexpressSearchKeyword');
    const searchResults = document.getElementById('aliexpressSearchResults');
    const searchLoading = document.getElementById('aliexpressSearchLoading');
    const searchUrl = root.dataset.searchUrl;
    const defaultImage = root.dataset.defaultImage;

    const toggleModal = (show) => {
        if (!searchModal) {
            return;
        }
        searchModal.classList.toggle('show', show);
    };

    const searchProducts = () => {
        if (!searchUrl || !searchKeyword) {
            return;
        }
        const keyword = searchKeyword.value.trim();
        if (!keyword) {
            alert('Please enter a search keyword');
            return;
        }
        searchResults.innerHTML = '';
        searchLoading.style.display = 'block';
        fetch(`${searchUrl}?keyword=${encodeURIComponent(keyword)}`)
            .then((response) => response.json())
            .then((data) => {
                searchLoading.style.display = 'none';
                if (data.success && Array.isArray(data.data) && data.data.length) {
                    data.data.forEach((product) => {
                        const item = document.createElement('div');
                        item.className = 'aliexpress-search-result-item';
                        item.addEventListener('click', () => selectProduct(product));
                        item.innerHTML = `
                            <img src="${product.ImageUrl || defaultImage}" alt="${product.ProductTitle}">
                            <div class="aliexpress-search-result-title">${product.ProductTitle}</div>
                            <div class="aliexpress-search-result-price">${product.Currency || 'USD'} ${product.SalePrice || product.OriginalPrice || 'N/A'}</div>
                        `;
                        searchResults.appendChild(item);
                    });
                } else {
                    searchResults.innerHTML = '<p class="text-center text-muted">No products found. Try a different keyword.</p>';
                }
            })
            .catch((error) => {
                searchLoading.style.display = 'none';
                console.error('Error:', error);
                searchResults.innerHTML = '<p class="text-center text-danger">Error searching products. Please try again.</p>';
            });
    };

    const selectProduct = (product) => {
        productIdField.value = product.ProductId;
        productDataField.value = JSON.stringify(product);
        selectedImage.src = product.ImageUrl || defaultImage;
        selectedTitle.textContent = product.ProductTitle;
        selectedPrice.textContent = `${product.Currency || 'USD'} ${product.SalePrice || product.OriginalPrice || 'N/A'}`;
        selectedId.textContent = product.ProductId;
        selectedProductBlock.style.display = 'block';
        noSelectionBlock.style.display = 'none';
        toggleModal(false);
    };

    const removeProduct = () => {
        if (!confirm('Are you sure you want to remove the AliExpress product association?')) {
            return;
        }
        productIdField.value = '';
        productDataField.value = '';
        selectedProductBlock.style.display = 'none';
        noSelectionBlock.style.display = 'block';
    };

    const initState = () => {
        if (productIdField.value) {
            selectedProductBlock.style.display = 'block';
            noSelectionBlock.style.display = 'none';
        }
    };

    root.addEventListener('click', (evt) => {
        const action = evt.target.closest('[data-action]');
        if (!action) {
            return;
        }
        evt.preventDefault();
        switch (action.dataset.action) {
            case 'open-search':
                toggleModal(true);
                break;
            case 'close-search':
                toggleModal(false);
                break;
            case 'remove-product':
                removeProduct();
                break;
            case 'search-products':
                searchProducts();
                break;
        }
    });

    searchKeyword?.addEventListener('keypress', (evt) => {
        if (evt.key === 'Enter') {
            evt.preventDefault();
            searchProducts();
        }
    });

    initState();
})();

