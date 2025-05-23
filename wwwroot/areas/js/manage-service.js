/* Working Time */
function bootstrapMultiselectInitial() {
    $('#working-times').multiselect({
        includeSelectAllOption: true,
        maxHeight: 200,
        selectAllText: 'Chọn tất cả',
        buttonText: function (options, select) {
            if (options.length === 0) {
                return 'Chọn giờ làm việc';
            } else if (options.length > 3) {
                return options.length + ' giờ làm việc đã chọn';
            } else {
                let selected = '';
                options.each(function () {
                    selected += $(this).text() + ', ';
                });
                return selected.substr(0, selected.length - 2);
            }
        }
    });
}

/* Service Type */
function suggestServiceTypes() {
    let content = '';
    let inputValue = $("#service-type").val().toLowerCase();
    if (inputValue.length === 0) {
        listServiceType.forEach(function (type) {
            content += `<li><a onclick="selectServiceType(this)">${type}</a></li>`;
        });
    } else {
        listServiceType.forEach(function (type) {
            let typeUnaccent = removeVietnameseAccents(type.toLowerCase());
            let inputUnaccent = removeVietnameseAccents(inputValue);
            if (typeUnaccent.includes(inputUnaccent)) {
                content += `<li><a onclick="selectServiceType(this)">${type}</a></li>`;
            }
        });
    }

    if (content.length === 0) {
        hiddenSuggest('list-service-type');
    } else {
        $("#list-service-type").empty();
        $("#list-service-type").html(content);
        $("#list-service-type").addClass("display");
    }
}

function selectServiceType(element) {
    $("#service-type").val($(element).text());
}

function hiddenSuggest(elementId) {
    $("#" + elementId).removeClass("display");
}

function removeVietnameseAccents(str) {
    return str.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}

/* Subdescription */
function addSentence(event) {
    if (event.key === 'Enter') {
        event.preventDefault();

        let input = document.getElementById("supdescriptionInput");
        let container = document.getElementById("supdescriptionContainer");
        let sentence = input.value.trim();
        if (sentence.length > 0) {
            let div = document.createElement("div");
            div.className = 'subdescription-tag';
            div.textContent = sentence;

            let removeBtn = document.createElement("button");
            removeBtn.textContent = "x";
            removeBtn.addEventListener("click", function () {
                container.removeChild(div);
                updateSupdescription();
            });

            div.appendChild(removeBtn);
            container.appendChild(div);

            input.value = '';
            updateSupdescription();
        }
    }
}

function updateSupdescription() {
    let content = '<ul>';
    let container = $("#supdescriptionContainer");

    container.children('.subdescription-tag').each(function () {
        content += `<li>${$(this).text().slice(0, -1).trim()}</li>`;
    });

    content += '</ul>';

    service.subdescription = content;
}

/* Images */
function uploadImages(event) {
    let files = event.target.files;
    let previewContainer = document.getElementById('preview-image');

    for (let file of files) {
        let div = document.createElement('div');
        div.className = 'item';

        let reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function (e) {
            let img = document.createElement('img');
            img.src = e.target.result;
            img.alt = file.name;

            div.appendChild(img);

            let removeBtn = document.createElement('button');
            removeBtn.className = 'btn-remove';
            removeBtn.innerHTML = '<i class="icon-x-circle"></i>';
            removeBtn.onclick = function () {
                previewContainer.removeChild(div);
                updateListImage();
                $("#image").val('');
            }

            div.appendChild(removeBtn);
            previewContainer.appendChild(div);

            updateListImage();
        }
    }
}

function updateListImage() {
    let previewContainer = document.getElementById('preview-image');
    let images = previewContainer.getElementsByTagName('img');
    let imageList = [];

    for (let image of images) {
        imageList.push(image.src);
    }

    service.images = imageList;
}


/* Pet Weight */
function suggestPetWeights() {
    let content = '';
    let inputValue = $("#pet-weight").val().toLowerCase();
    if (inputValue.length === 0) {
        listPetWeight.forEach(function (weight) {
            content += `<li><a onclick="selectPetWeight(this)">${weight}</a></li>`;
        });
    } else {
        listPetWeight.forEach(function (weight) {
            let weightUnaccent = removeVietnameseAccents(weight.toLowerCase());
            let inputUnaccent = removeVietnameseAccents(inputValue);
            if (weightUnaccent.includes(inputUnaccent)) {
                content += `<li><a onclick="selectPetWeight(this)">${weight}</a></li>`;
            }
        });
    }

    if (content.length === 0) {
        hiddenSuggest('list-pet-weight');
    } else {
        $("#list-pet-weight").empty();
        $("#list-pet-weight").html(content);
        $("#list-pet-weight").addClass("display");
    }
}

function selectPetWeight(element) {
    $("#pet-weight").val('');
    $("#pet-weight").val($(element).text());
}


/* Service Option */
function validationOfServiceOption(actionBtn) {
    let petType = $("#pet-type").val();

    let petWeight = $("#pet-weight").val().trim();
    if (petWeight === '') {
        isValidServiceOption = false;
        $("#error-pet-weight").text("Khối lượng không được để trống");
    } else if (petWeight.length >= 50) {
        isValidServiceOption = false;
        $("#error-pet-weight").text("Khối lượng không được vượt quá 50 ký tự");
    } else {
        $("#error-pet-weight").text('');
    }

    let price = $("#price").val().trim();
    if (price === '') {
        isValidServiceOption = false;
        $("#error-price").text("Giá dịch vụ không được để trống");
    } else {
        let priceParsed = Number(price);
        if (!isNaN(priceParsed)) {
            if (priceParsed < 0) {
                isValidServiceOption = false;
                $("#error-price").text("Giá dịch vụ phải là số dương");
            } else {
                $("#error-price").text('');
            }
        } else {
            isValidServiceOption = false;
            $("#error-price").text("Giá dịch vụ phải là một số");
        }
    }

    if (isValidServiceOption && actionBtn === 'create' && service.serviceOptions.length > 0) {
        for (let option of service.serviceOptions) {
            if (option.petType === petType && option.weight === petWeight) {
                isValidServiceOption = false;
                alert("Tùy chọn dịch vụ này đã tồn tại. Vui lòng thay đổi loại thú cưng hoặc khối lượng để thêm mới.");
                break;
            }
        }
    }

    if (isValidServiceOption && actionBtn === 'update' && service.serviceOptions.length > 0) {
        for (let option of service.serviceOptions) {
            if (option.petType === petType && option.weight === petWeight && option.serviceOptionId != currentServiceOptionId) {
                isValidServiceOption = false;
                alert("Tùy chọn dịch vụ này đã tồn tại. Vui lòng thay đổi loại thú cưng hoặc khối lượng để thêm mới.");
                break;
            }
        }
    }
}

/* Service */
function validationOfService(actionBtn) {
    service.name = $("#service-name").val().trim();
    if (service.name === '') {
        isValidService = false;
        $("#error-service-name").text("Tên dịch vụ không được để trống");
    } else if (service.name.length >= 200) {
        isValidService = false;
        $("#error-service-name").text("Tên dịch vụ không được vượt quá 200 ký tự");
    } else {
        $('#error-service-name').text('');
    }

    service.type = $("#service-type").val().trim();
    if (service.type === '') {
        isValidService = false;
        $("#error-service-type").text("Tên loại dịch vụ không được để trống");
    } else if (service.type.length >= 200) {
        isValidService = false;
        $("#error-service-type").text("Tên loại dịch vụ không được vượt quá 200 ký tự");
    } else {
        $('#error-service-type').text('');
    }

    service.workingTimes = $('#working-times').val();
    if (service.workingTimes.length === 0) {
        isValidService = false;
        $("#error-working-times").text("Cần chọn ít nhất một giờ làm việc");
    } else {
        $("#error-working-times").text('');
    }

    if (service.subdescription === '' || service.subdescription === '<ul></ul>') {
        isValidService = false;
        $("#error-subdescription").text("Cần ít nhất một mô tả phụ của dịch vụ");
    } else {
        $("#error-subdescription").text('');
    }

    if (service.images.length === 0) {
        isValidService = false;
        $("#error-images").text("Cần ít nhất một ảnh của dịch vụ");
    } else {
        $("#error-images").text('');
    }

    if (service.serviceOptions.length === 0) {
        isValidService = false;
        alert("Cần thêm ít nhất một tùy chọn của dịch vụ");
    } else {
        $("#error-service-option").text('');
    }

    if (actionBtn === 'update') {
        service.serviceId = $("#serviceId").val();
        service.isDelete = $(".radio-buttons").find(".service-status:checked").val();

        if (service.isDelete === 'false') {
            let isAllOptionDeleted = true;
            for (let option of service.serviceOptions) {
                if (option.isDelete === false || option.isDelete === 'false') {
                    isAllOptionDeleted = false;
                    break;
                }
            }

            if (isAllOptionDeleted === true) {
                isValidService = false;
                alert("Bạn đang chọn dịch vụ này còn kinh doanh. Do vậy, cần ít nhất một tùy chọn của dịch vụ này là còn kinh doanh");
            }
        }
    }
}
