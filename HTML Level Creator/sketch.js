const KeyboardToTool = {
    84: "toggler",
    67: "none",
    83: "star",
    68: "ladder",
    73: "inverter",
    75: "key",
    76: "lock"
}

const KeyColors = ["red", "green", "purple", "gold", undefined]
const NamesToHex = {
    "red": "#ff0000",
    "green": "#00ff00",
    "purple": "#9c00ff",
    "gold": "#ffc500"
}
const BoxColorClosed = "#602a00";
const BoxColorOpen = "#903610";
const StarColor = "#03a5fc";
const LadderColor = "#291600";
const InverterColor = "#ffff00";
const MaxFloors = 15;
const MaxColumns = 31;

const floorSlider = document.getElementById("floors");
const columnSlider = document.getElementById("columns");
const outputArea = document.getElementById("output");
const map = [];
var tool = "toggler";

function setup() {
    const cnv = createCanvas(1280, 720);
    cnv.parent("canvas_container");
    
    for (let floor = 0; floor < MaxFloors; floor++) {
        map.push([]);

        for (let column = 0; column < MaxColumns; column++) {
            map[floor].push([]);

            for (let row = 0; row < 2; row++) {
                map[floor][column].push({"open": false});
            }
        }
    }
}

function draw() {
    background(172);

    for (let floor = 0; floor < floorSlider.value; floor++) {
        for (let column = 0; column < columnSlider.value; column++) {
            for (let row = 0; row < 2; row++) {
                drawBox(floor, column, row);
            }
        }
    }
}

function drawBox(floor, column, row) {
    const columns = columnSlider.value;
    const floors = floorSlider.value;
    const columnWidth = width/columns;
    const columnHeight = height/floors;

    const currentBox = map[floor][column][row];
    const boxWidth = columnWidth * 0.7;
    const boxHeight = columnHeight * 0.3;
    const boxX = column * columnWidth + boxWidth/4;
    const boxY = floor * columnHeight + boxHeight/2 + row * columnHeight / 2.5;
    const centerX = boxX + boxWidth/2;
    const centerY = boxY + boxHeight/2;

    fill(currentBox.open ? BoxColorOpen : BoxColorClosed);                
    rect(boxX, boxY, boxWidth, boxHeight);

    switch (currentBox.contents) {
        case "star":
            fill(StarColor);
            ellipse(centerX, centerY, boxWidth/2, boxHeight/2);
            break;
        case "ladder":
            fill(LadderColor);
            rect(centerX - boxWidth/6, centerY, boxWidth/3, boxHeight*2);
            break;
        case "inverter":
            fill(InverterColor);
            rect(boxX + boxWidth/4, boxY + boxHeight/4, boxWidth/2, boxHeight/2);
            break;
        case "key":
            fill(NamesToHex[currentBox.key_color]);
            ellipse(centerX, centerY, boxWidth/2, boxHeight/2);
            break;
    }

    if (currentBox.lock_color != undefined) {
        fill(NamesToHex[currentBox.lock_color]);
        rect(boxX + boxWidth/4, boxY - boxHeight/3, boxWidth/2, boxHeight/2);
    }
}

function generateJSON() {
    const columns = columnSlider.value;
    const floors = floorSlider.value;
    let out = {
        "columns": int(columns),
        "floors": int(floors),
        "boxes": []
    };

    for(let floor = 0; floor < floors; floor++) {
        out.boxes.push({"floor": []});
        for(let column = 0; column < columns; column++) {
            out.boxes[floor].floor.push({
                "top": map[floor][column][0],
                "bottom": map[floor][column][1],
            });
        }
    }

    outputArea.value = JSON.stringify(out);
}

function screenspaceToGameCoords(x, y) {
    const columns = columnSlider.value;
    const floors = floorSlider.value;
    const columnWidth = width/columns;
    const columnHeight = height/floors;

    const c = Math.floor(x / columnWidth);
    const f = Math.floor(y / columnHeight);
    const r = (y/columnHeight) % 1 > 0.5 ? 1 : 0;

    if (c < 0 || c > columns-1 || f < 0 || f > floors-1) return null;
    return {floor: f, column: c, row: r}
}

function mousePressed() {
    const coords = screenspaceToGameCoords(mouseX, mouseY);
    if (coords === null) return;
    const selectedBox = map[coords.floor][coords.column][coords.row];

    switch (tool) {
        case "toggler":
            selectedBox.open = !selectedBox.open;
            break;
        case "key":
            selectedBox.contents = tool;
            // if (selectedBox.key_color === undefined) {
            //     selectedBox.key_color = 0;
            // }
            // else if (selectedBox.key_color == KeyColors.length - 1) {
            //     selectedBox.key_color = 0
            // }
            // else {
            //     selectedBox.key_color += 1;
            // }
            selectedBox.key_color = getNextKeyColor(selectedBox.key_color);
            if (selectedBox.key_color === undefined) {
                selectedBox.contents = undefined;
            }
            break;
        case "lock":
            // if (selectedBox.lock_color === undefined) {
            //     selectedBox.lock_color = 0;
            // }
            // else if (selectedBox.lock_color === KeyColors.length) {
            //     selectedBox.lock_color = 0;
            // }
            // else {
            //     selectedBox.lock_color += 1;
            // }
            selectedBox.lock_color = getNextKeyColor(selectedBox.lock_color);
            break;
        case "ladder":
            if (coords.row === 1) {
                selectedBox.contents = tool;
            }
            break;
        case "none": 
            selectedBox.contents = tool;
            selectedBox.key_color = undefined;
            break;
        default:
            selectedBox.contents = tool;
            break;
    }
    generateJSON();
}

function getNextKeyColor(color) {
    let nextIndex = KeyColors.indexOf(color) + 1;

    if (nextIndex === KeyColors.length) {
        return KeyColors[0];
    }
    return KeyColors[nextIndex];
}

function keyPressed() {
    const t = KeyboardToTool[keyCode];
    if (t !== undefined) {
        tool = t;
    }
}