$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $("#messages").DataTable({
        ajax: {
            url: "/Admin/Messages/GetAll",
        },
        columns: [
            { data: "name", width: "25%" },
            { data: "email", width: "25%" },
            { data: "message", width: "25%" },
            {
                data: "id",
                render: function (data) {
                    return `
                            <a onclick=Delete("/Admin/Messages/Delete/${data}") class="btn" data-bs-toggle="modal" data-bs-target="#Modal">
                                <i class="fa fa-trash"></i> &nbsp;
                            </a>
                            </div>`;
                },
                width: "20%",
            },
        ],
    });
}
