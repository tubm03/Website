function exportToExcel(data, nameFile) {
    // Tạo workbook và worksheet từ dữ liệu
    const worksheet = XLSX.utils.json_to_sheet(data);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

    // Xuất file Excel
    XLSX.writeFile(workbook, nameFile + '.xlsx');
}

function buttonExportFile(dataJson, nameFile) {
    const jsonString = JSON.stringify(dataJson);
    const arrayOfObjects = JSON.parse(jsonString);
    // Kiểm tra kết quả
    console.log(arrayOfObjects);
    exportToExcel(arrayOfObjects, nameFile);
}