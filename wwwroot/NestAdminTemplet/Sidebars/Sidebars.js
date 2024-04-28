let btn = document.querySelector(".fatt");
let sidebar = document.querySelector(".sidebar2");

btn.addEventListener("click", () => {
  sidebar.classList.toggle("close");
});

let arrows = document.querySelectorAll(".arrow2");
for (var i = 0; i < arrows.length; i++) {
  arrows[i].addEventListener("click", (e) => {
    let arrowParent = e.target.parentElement.parentElement;

    arrowParent.classList.toggle("show");
  });
}
