//HtmlSanitizer default doesnt inject classes so set quill to use inline style for align
const AlignStyle = Quill.import('attributors/style/align');
Quill.register(AlignStyle, true);
//adds quill to the editor div with below settings
const quill = new Quill('#editor', {
    theme: 'snow',
    modules: {
        toolbar: [
            [{ header: [1, 2, 3, 4, 5, 6, false] }],
            ['bold', 'italic', 'underline', 'strike'],
            ['blockquote', 'code-block'],
            [{ 'align': [] }],
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            [{ 'color': [] }, { 'background': [] }],
            ['clean'],
        ]
    }
});
//the textarea note element
const noteInput = document.getElementById('Note');
//for edit dump html into quill
if (noteInput.value) {
    quill.clipboard.dangerouslyPasteHTML(noteInput.value);
}

//limit the submitted HTML to 5000 chars (matches server model max length)
const limit = 5000;
const characterCount = document.querySelector('.character-count');

function getHtmlLength() {
    return quill.root.innerHTML.length;
}

function updateCharacterCount() {
    if (!characterCount) {
        return;
    }

    const remaining = Math.max(0, limit - getHtmlLength());
    characterCount.innerText = remaining + ' Characters Remaining';
}

//Note even when Quill is empty, there is still a blank line represented by '\n', so getLength will return 1.
//The last editable character index is therefore getLength() - 2
function enforceHtmlLengthLimit() {
    //remove text from the end until rendered html fits in DB/model limit
    while (getHtmlLength() > limit && quill.getLength() > 1) {
        quill.deleteText(quill.getLength() - 2, 1, 'silent');
    }
}

quill.on('text-change', function () {
    enforceHtmlLengthLimit();
    updateCharacterCount();
});

//initialize current count and enforce for existing content
enforceHtmlLengthLimit();
updateCharacterCount();


document.getElementById('note-form').addEventListener('submit', function (event) {
    enforceHtmlLengthLimit();
    //before submit take quill html and add it to textarea element thats bound to the model
    noteInput.value = quill.root.innerHTML;
    //Should never really happen but a defensive check anyway
    if (noteInput.value.length > limit) {
        event.preventDefault();
        toastr.error('Note exceeds 5000 character storage limit. Please shorten your note.');
    }
});