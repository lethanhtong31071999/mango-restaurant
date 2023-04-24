$(document).ready(function () {
    loadProductTable();
});

function loadProductTable() {
    $('#product-table-admin').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        pagingType: "simple_numbers",
        "ajax": {
            url: $('#product-table-admin').data('url'),
            type: "POST",
        },
        scrollY: '500px',
        scrollCollapse: true,
        "columns": [
            { "data": "productId", "width": "5%" },
            { "data": "name", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "categoryName", "width": "15%" },
            {
                "data": "description",
                "width": "20%",
                "render": function (description) {
                    return `
                    <p class="text-secondary" style="display:-webkit-box;overflow:hidden;-webkit-line-clamp:3;-webkit-box-orient:vertical;">
                        ${description}
                    </p>
                `;
                }
            },
            {
                "data": "imageUrl",
                "width": "10%",
                "render": function (imageUrl) {
                    return `
                    <div class="btn-group d-flex justify-content-center" role="group">
                        <img src="${imageUrl}" alt="product" class="img-thumbnail" style="aspect-ratio: 16:9;">
                    </div>
                `;
                }
            },
            {
                "data": "productId",
                "width": "20%",
                "render": function (productId) {
                    const upsertUrl = $('#product-table-admin').data('upsert-url');
                    const deleteUrl = $('#product-table-admin').data('delete-url');

                    return `
                    <div class="d-flex justify-content-center" role="group">
                        <a class="btn btn-primary flex-grow-1 mx-1" href="${upsertUrl}/${productId}" >
                            <i class="bi bi-pencil-square"></i>
                        </a>
                        <a class="btn btn-danger flex-grow-1 mx-1" href="${deleteUrl}/${productId}" >
                            <i class="bi bi-trash"></i>
                        </a>
                    </div>
                `;
                }
            }
        ]
    });
}