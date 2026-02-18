// HtmlSanitizer default doesnt inject classes so set quill to use inline style for align
const AlignStyle = Quill.import('attributors/style/align');
Quill.register(AlignStyle, true);
//tool bar config
const defaultToolbar = [
    [{ header: [1, 2, 3, 4, 5, 6, false] }],
    ['bold', 'italic', 'underline', 'strike'],
    ['link'],
    ['blockquote', 'code-block'],
    [{ align: [] }],
    [{ list: 'ordered' }, { list: 'bullet' }],
    [{ 'indent': '-1' }, { 'indent': '+1' }], 
    [{ 'script': 'sub' }, { 'script': 'super' }],
    [{ color: [] }, { background: [] }],
    ['clean']
];

function initQuillEditor(container) {
    const editorEl = container.querySelector('.quill-editor');
    if (!editorEl) return;

    const inputSelector = container.dataset.inputSelector;
    const formSelector = container.dataset.formSelector;
    const counterSelector = container.dataset.counterSelector;
    const limit = Number(container.dataset.limit || 5000);
    //the textarea note element
    const noteInput = inputSelector ? document.querySelector(inputSelector) : null;
    const form = formSelector ? document.querySelector(formSelector) : null;
    const counter = counterSelector ? document.querySelector(counterSelector) : null;

    if (!noteInput || !form) return;

    //adds quill to the editor div
    const quill = new Quill(editorEl, {
        theme: 'snow',
        modules: { toolbar: defaultToolbar }
    });
    //for edit dump html into quill
    if (noteInput.value) {
        quill.clipboard.dangerouslyPasteHTML(noteInput.value);
    }

    const getHtmlLength = () => {
        return quill.root.innerHTML.length;
    }

    const updateCharacterCount = () => {
        if (!counter) return;
        const remaining = Math.max(0, limit - getHtmlLength());
        counter.innerText = `${remaining} Characters Remaining`;
    }

    //Note even when Quill is empty, there is still a blank line represented by '\n', so getLength will return 1.
    //The last editable character index is therefore getLength() - 2
    const enforceHtmlLengthLimit = () => {
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

    form.addEventListener('submit', function (event) {
        enforceHtmlLengthLimit();
        //before submit take quill html and add it to textarea element thats bound to the model
        noteInput.value = quill.root.innerHTML;
        if (noteInput.value.length < 20) {
            event.preventDefault();
            toastr.error('Not enough characters.');
        }
        //Should never really happen but a defensive check anyway
        if (noteInput.value.length > limit) {
            event.preventDefault();
            toastr.error('Exceeds character storage limit. Please shorten.');
        }
    });
}

for (const container of document.querySelectorAll('[data-quill-editor]')) {
    initQuillEditor(container);
}
